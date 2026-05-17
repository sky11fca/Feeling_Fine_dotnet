#!/bin/bash

# ==============================================================================
# AWS Deployment Script for FeelingFine (High Speed Version)
# ==============================================================================
# This script ensures success and speed by:
# 1. Transferring pre-built images from GHCR to ECR (No slow local builds).
# 2. Automatically handling CloudFormation stack lifecycles.
# ==============================================================================

# Exit on error
set -e

# Load environment variables
if [ -f .env.aws ]; then
  echo "Loading configuration from .env.aws..."
  export $(grep -v '^#' .env.aws | xargs)
fi

# Configuration
AWS_REGION=${AWS_REGION:-"eu-north-1"}
STACK_NAME=${STACK_NAME:-"feelingfine-stack"}

# Fallback: Guess GitHub Owner from git if not provided
if [ -z "$GITHUB_REPOSITORY_OWNER" ]; then
    echo "Warning: GITHUB_REPOSITORY_OWNER not set. Trying to guess from git..."
    GITHUB_REPOSITORY_OWNER=$(git config --get remote.origin.url | sed -E 's|.*/([^/]+)/.*|\1|' | sed 's/.*://')
    echo "Guessed owner: $GITHUB_REPOSITORY_OWNER"
fi

if [ -z "$GHCR_PAT" ]; then
    echo "ERROR: GHCR_PAT is missing in .env.aws. Please provide a GitHub PAT to pull your images."
    exit 1
fi

# 1. Unlock AWS (Cleanup)
echo "Step 1: Checking for stuck stacks in $AWS_REGION..."
STACK_STATUS=$(aws cloudformation describe-stacks --stack-name "$STACK_NAME" --region "$AWS_REGION" --query "Stacks[0].StackStatus" --output text 2>/dev/null || echo "NOT_FOUND")

if [[ "$STACK_STATUS" == "ROLLBACK_COMPLETE" || "$STACK_STATUS" == "ROLLBACK_FAILED" || "$STACK_STATUS" == "CREATE_IN_PROGRESS" ]]; then
    echo "Stack is stuck ($STACK_STATUS). Deleting to start fresh..."
    aws cloudformation delete-stack --stack-name "$STACK_NAME" --region "$AWS_REGION"
    echo "Waiting for deletion to finish (this takes 2 mins)..."
    aws cloudformation wait stack-delete-complete --stack-name "$STACK_NAME" --region "$AWS_REGION"
    echo "Cleanup complete."
fi

# 2. Create Repositories if they don't exist
echo "Step 2: Ensuring ECR Repositories exist..."
for svc in "feelingfine-dotnet-api" "feelingfine-python-ai" "feelingfine-web-ui"; do
    aws ecr describe-repositories --repository-names "$svc" --region "$AWS_REGION" &>/dev/null || \
    aws ecr create-repository --repository-name "$svc" --region "$AWS_REGION" --output none
done

# 3. Transfer Images from GitHub to AWS (High Speed)
echo "Step 3: Transferring pre-built images from GitHub to AWS..."
ACCOUNT_ID=$(aws sts get-caller-identity --query "Account" --output text)
ECR_REGISTRY="$ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"
GH_OWNER_LOWER=$(echo "${GITHUB_REPOSITORY_OWNER}" | tr '[:upper:]' '[:lower:]')

# Login to both registries
echo "$GHCR_PAT" | docker login ghcr.io -u "$GITHUB_REPOSITORY_OWNER" --password-stdin
aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$ECR_REGISTRY"

for svc in "feelingfine-dotnet-api" "feelingfine-python-ai"; do
    echo "Transferring $svc (linux/amd64)..."
    docker pull --platform linux/amd64 "ghcr.io/${GH_OWNER_LOWER}/$svc:latest"
    
    # Create a wrapper to flatten the image and strip manifest list metadata
    echo "FROM ghcr.io/${GH_OWNER_LOWER}/$svc:latest" > Dockerfile.transfer
    docker build --platform linux/amd64 -t "$ECR_REGISTRY/$svc:latest" -f Dockerfile.transfer .
    rm Dockerfile.transfer
    
    docker push "$ECR_REGISTRY/$svc:latest"
done

# 4. Deploy Infrastructure
echo "Step 4: Creating the Cloud (this takes 5-8 mins)..."
aws cloudformation deploy \
    --template-file aws-infrastructure.yml \
    --stack-name "$STACK_NAME" \
    --capabilities CAPABILITY_IAM \
    --region "$AWS_REGION" \
    --parameter-overrides \
        DatabaseConnectionString="$DB_CONNECTION_STRING" \
        JwtSecret="$JWT_SECRET" \
        HuggingFaceToken="$HF_TOKEN"

# 5. Build and Push WebUI (Requires ALB DNS)
echo "Step 5: Finalizing Web UI with ALB DNS..."
ALB_DNS=$(aws cloudformation describe-stacks --stack-name "$STACK_NAME" --region "$AWS_REGION" --query "Stacks[0].Outputs[?OutputKey=='ALBDns'].OutputValue" --output text)

docker build --platform linux/amd64 -t "$ECR_REGISTRY/feelingfine-web-ui:latest" \
    --build-arg API_URL="http://$ALB_DNS" \
    --build-arg AI_URL="http://$ALB_DNS" \
    -f WebApi/WebApi/Dockerfile ./WebApi
docker push "$ECR_REGISTRY/feelingfine-web-ui:latest"

# 6. Force ECS to see the new WebUI
aws ecs update-service --cluster feelingfine --service web-ui --force-new-deployment --region "$AWS_REGION" --output none

echo "=============================================================================="
echo "DEPLOYMENT SUCCESSFUL!"
echo "=============================================================================="
echo "Your website is live at: http://$ALB_DNS"
echo "=============================================================================="
