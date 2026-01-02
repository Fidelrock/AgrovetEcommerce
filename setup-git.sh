#!/bin/bash
# Agrovet Git Setup Script

echo "?? Setting up Git repository for AgrovetEcommerce..."

# Initialize git (already done)
echo "? Git repository initialized"

# Configure git user
git config user.email "fidelisodhiambo254@gmail.com"
git config user.name "Fidelrock"
echo "? Git user configured"

# Add all files
git add .
echo "? Files staged"

# Create initial commit
git commit -m "Initial commit: Clean Architecture setup with logging and database

- Domain layer with entities (Category, Product, Order, OrderItem)
- Application layer with interfaces and services
- Infrastructure layer with EF Core context
- API layer with Serilog structured logging
- Zero-noise logging policy implemented
- SQLite database configured
- Swagger/OpenAPI documentation"

echo "? Initial commit created"

# Set main branch
git branch -M main
echo "? Main branch configured"

# Add remote
git remote add origin https://github.com/Fidelrock/AgrovetEcommerce.git
echo "? Remote origin configured"

echo ""
echo "?? Ready to push! Run:"
echo "   git push -u origin main"
echo ""
