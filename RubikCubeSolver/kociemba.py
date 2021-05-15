import sys
from rubik_solver import utils
cube = sys.argv[1]

print(','.join(str(x) for x in utils.solve(cube, 'Kociemba')))
