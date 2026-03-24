FROM python:3.11-slim

# Set the working directory inside the container
WORKDIR /app

# Install dependencies
# (Optional system packages can be added here if transformers or other packages require them)
RUN apt-get update && apt-get install -y \
    --no-install-recommends \
    curl build-essential \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the rest of the application code
COPY . .

EXPOSE 8000

# Command to run the application (Adjust 'main:app' if your entrypoint differs)
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]