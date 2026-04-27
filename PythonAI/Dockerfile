FROM python:3.11-slim

# Set the working directory inside the container
WORKDIR /app

# Install dependencies
RUN apt-get update && apt-get install -y \
    --no-install-recommends \
    curl build-essential \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN useradd -m appuser
USER appuser

COPY --chown=appuser:appuser requirements.txt .
RUN pip install --user --no-cache-dir -r requirements.txt

# Copy specifically instead of COPY . .
COPY --chown=appuser:appuser main.py .
COPY --chown=appuser:appuser api/ ./api/
COPY --chown=appuser:appuser core/ ./core/
COPY --chown=appuser:appuser model/ ./model/
COPY --chown=appuser:appuser service/ ./service/

ENV PATH="/home/appuser/.local/bin:${PATH}"

EXPOSE 8000

# Command to run the application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
