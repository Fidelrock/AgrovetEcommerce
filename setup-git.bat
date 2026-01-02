@echo off
REM Agrovet Git Setup Script for Windows

echo.
echo ?? Setting up Git repository for AgrovetEcommerce...
echo.

REM Configure git user
git config user.email "fidelisodhiambo254@gmail.com"
git config user.name "Fidelrock"
echo ? Git user configured

REM Add all files
git add .
echo ? Files staged

REM Create initial commit
git commit -m "Initial commit: Clean Architecture setup with logging and database

- Domain layer with entities (Category, Product, Order, OrderItem)
- Application layer with interfaces and services
- Infrastructure layer with EF Core context
- API layer with Serilog structured logging
- Zero-noise logging policy implemented
- SQLite database configured
- Swagger/OpenAPI documentation"

echo ? Initial commit created

REM Set main branch
git branch -M main
echo ? Main branch configured

REM Add remote
git remote add origin https://github.com/Fidelrock/AgrovetEcommerce.git
echo ? Remote origin configured

echo.
echo ?? Ready to push! Run:
echo    git push -u origin main
echo.
pause
