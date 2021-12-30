$x=pwd
$y=$x.Path
$env:PYTHONPATH="$y\Test"
python3 -m unittest Test/Smoke.py