# expToBook
Tool to make a BIN book from an "eman-like" EXP file<p>

Prerequisites :<br>
rename BUREAU.ini to YOUR-COMPUTER-NAME.ini<br>
set moteurEXP to path_to_your_eman_engine.exe<br>
set fichierEXP to path_to_your_experience_file.exp<p>

# Most common scenario
1°) enter an UCI string :<br>
![1 enter UCI string](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/1.%20enter%20UCI%20string.jpg)<p>

2°) conversion in progress :<br>
![2 converting](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/2.%20converting.jpg)<p>

How it works ?<br>
At best, it searches until the [100th ply](https://github.com/chris13300/expToBook/blob/main/expToBook/modMain.vb#L55) from the EXP file and it adds until [3 bestmoves](https://github.com/chris13300/expToBook/blob/main/expToBook/modMain.vb#L155) per position. The percentages of the book's moves are calculated according to the quality's values of bestmoves from the EXP file.<br>
![expToBook](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/expToBook.jpg)<br>
