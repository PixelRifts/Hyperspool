@echo off
set commit-msg = %1
git add .
git status
git commit -m commit-msg
git push