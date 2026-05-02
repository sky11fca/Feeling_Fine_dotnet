FROM python:3.11-slim

# Set the working directory inside the container
WORKDIR /app

# Install dependencies
RUN apt-get update && apt-get install -y \
    --no-install-recommends \
    curl build-essential \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user and group
RUN groupadd -r appgroup && useradd --no-log-init -r -g appgroup -d /home/appuser -m appuser

# Copy and install Python dependencies
# Copying as root and setting read-only permissions for security
COPY --chown=root:root --chmod=555 requirements.txt .

# Switch to the non-root user for pip install so it installs to /home/appuser/.local
USER appuser
RUN pip install --user --no-cache-dir --only-binary :all: -r requirements.txt

# Switch back to root to copy the rest of the application code securely
USER root
COPY --chown=root:root --chmod=555 main.py .
COPY --chown=root:root --chmod=555 api/ ./api/
COPY --chown=root:root --chmod=555 core/ ./core/
COPY --chown=root:root --chmod=555 model/ ./model/
COPY --chown=root:root --chmod=555 service/ ./service/

# Switch back to the non-root user for execution
USER appuser

ENV PATH="/home/appuser/.local/bin:${PATH}"

EXPOSE 8000

# Command to run the application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
