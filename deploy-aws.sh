#!/bin/bash

# ==============================================================================
# AWS Deployment Script for FeelingFine (Smarter Cleanup)
# ==============================================================================
# This script provisions AWS resources using CloudFormation.
# It automatically handles stuck/failed stacks by deleting them before retry.
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

# --- New: Auto-Cleanup Logic ---
echo "Checking existing stack status in $AWS_REGION..."
# Use || true to prevent set -e from killing the script if stack is not found
STACK_STATUS=$(aws cloudformation describe-stacks --stack-name "$STACK_NAME" --region "$AWS_REGION" --query "Stacks[0].StackStatus" --output tsv 2>/dev/null || echo "NOT_FOUND")

if [[ "$STACK_STATUS" == *"IN_PROGRESS"* ]]; then
    echo "Stack is currently $STACK_STATUS. Waiting for it to reach a terminal state..."
    aws cloudformation wait stack-create-complete --stack-name "$STACK_NAME" --region "$AWS_REGION" 2>/dev/null || \
    aws cloudformation wait stack-update-complete --stack-name "$STACK_NAME" --region "$AWS_REGION" 2>/dev/null || \
    aws cloudformation wait stack-delete-complete --stack-name "$STACK_NAME" --region "$AWS_REGION" 2>/dev/null || \
    echo "Stack reached a terminal state."
    # Refresh status after waiting
    STACK_STATUS=$(aws cloudformation describe-stacks --stack-name "$STACK_NAME" --region "$AWS_REGION" --query "Stacks[0].StackStatus" --output tsv 2>/dev/null || echo "NOT_FOUND")
fi

if [[ "$STACK_STATUS" == "ROLLBACK_COMPLETE" || "$STACK_STATUS" == "ROLLBACK_FAILED" || "$STACK_STATUS" == "DELETE_FAILED" ]]; then
    echo "Stack is in a failed state ($STACK_STATUS). Deleting it to allow a clean retry..."
    aws cloudformation delete-stack --stack-name "$STACK_NAME" --region "$AWS_REGION"
    echo "Waiting for deletion to complete..."
    aws cloudformation wait stack-delete-complete --stack-name "$STACK_NAME" --region "$AWS_REGION"
    echo "Cleanup complete."
fi
# ------------------------------

echo "Step 1: Deploying CloudFormation Stack [$STACK_NAME]..."
echo "This will create VPC, ECR Repositories, ECS Cluster, and ALB..."
if ! aws cloudformation deploy \
    --template-file aws-infrastructure.yml \
    --stack-name "$STACK_NAME" \
    --capabilities CAPABILITY_IAM \
    --region "$AWS_REGION" \
    --parameter-overrides \
        DatabaseConnectionString="$DB_CONNECTION_STRING" \
        JwtSecret="$JWT_SECRET" \
        HuggingFaceToken="$HF_TOKEN"; then
    echo "ERROR: CloudFormation deployment failed."
    echo "Fetching last 10 error events..."
    aws cloudformation describe-stack-events \
        --stack-name "$STACK_NAME" \
        --region "$AWS_REGION" \
        --query 'StackEvents[?ResourceStatus==`CREATE_FAILED` || ResourceStatus==`UPDATE_FAILED`].[ResourceStatus, LogicalResourceId, ResourceStatusReason]' \
        --output table
    exit 1
fi

echo "Step 2: Fetching Outputs..."
ALB_DNS=$(aws cloudformation describe-stacks --stack-name "$STACK_NAME" --query "Stacks[0].Outputs[?OutputKey=='ALBDns'].outputValue" --output tsv --region "$AWS_REGION" 2>/dev/null || echo "ALB_NOT_READY")

echo "=============================================================================="
echo "Infrastructure Deployment Complete!"
echo "=============================================================================="
echo "ALB Public DNS: http://$ALB_DNS"
echo "=============================================================================="
echo "Next Step: Commit your changes and push to GitHub to trigger the CI/CD pipeline."
