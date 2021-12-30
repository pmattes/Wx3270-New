$pwd=Get-Location
$env:PYTHONPATH="$pwd\Test"
python3 -m unittest Test/Smoke.py