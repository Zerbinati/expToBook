# expToBook
Tool to make a BIN book from an "eman-like" EXP file<p>

Prerequisites :<br>
rename BUREAU.ini to YOUR-COMPUTER-NAME.ini<br>
set moteurEXP to path_to_your_eman_engine.exe<br>
set fichierEXP to path_to_your_experience_file.exp<p>

Most common scenario :<p>
1°) enter an UCI string :<br>
![1 enter UCI string](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/1.%20enter%20UCI%20string.jpg)<p>

2°) converting :<br>
![2 converting](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/2.%20converting.jpg)<p>

How it works ?<br>
It searches until the [100th ply](https://github.com/chris13300/expToBook/blob/main/expToBook/modMain.vb#L55) from the EXP file. At best, it adds until [3 bestmoves](https://github.com/chris13300/expToBook/blob/main/expToBook/modMain.vb#L155) per position. The percentages of the book's moves are calculated according to the quality's values of the 1 to 3 bestmoves from the EXP file.<br>
![expToBook](https://github.com/chris13300/expToBook/blob/main/expToBook/bin/Debug/expToBook.jpg)<br>
