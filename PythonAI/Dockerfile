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

# Change ownership of the working directory for the new user
RUN chown -R appuser:appgroup /app

# Switch to the non-root user
USER appuser

# Copy and install Python dependencies
COPY requirements.txt .
RUN pip install --user --no-cache-dir -r requirements.txt

# Copy the rest of the application code
COPY main.py .
COPY api/ ./api/
COPY core/ ./core/
COPY model/ ./model/
COPY service/ ./service/

ENV PATH="/home/appuser/.local/bin:${PATH}"

EXPOSE 8000

# Command to run the application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
