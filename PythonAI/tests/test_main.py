import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_app_starts():
    """Test that the application can be instantiated and the routes are included."""
    # Since we don't have a root / endpoint, we will just verify the routers are registered.
    routes = [route.path for route in app.routes]
    assert "/ai/review/" in routes
    assert "/ai/reply/" in routes
