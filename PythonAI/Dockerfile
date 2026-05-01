FROM python:3.11-slim

# Set the working directory inside the container
WORKDIR /app

# Install dependencies
RUN apt-get update && apt-get install -y \
    --no-install-recommends \
    curl build-essential \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user

COPY --chown=root:root --chmod=755 requirements.txt .
RUN pip install --user --no-cache-dir -r requirements.txt

# Copy specifically instead of COPY . .
COPY --chown=root:root --chmod=755 main.py .
COPY --chown=root:root --chmod=755 api/ ./api/
COPY --chown=root:root --chmod=755 core/ ./core/
COPY --chown=root:root --chmod=755 model/ ./model/
COPY --chown=root:root --chmod=755 service/ ./service/

ENV PATH="/home/appuser/.local/bin:${PATH}"

EXPOSE 8000

# Command to run the application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
