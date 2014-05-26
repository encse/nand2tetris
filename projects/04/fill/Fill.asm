// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input. 
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel. When no key is pressed, the
// program clears the screen, i.e. writes "white" in every pixel.

// Put your code here.
(BEGIN)
	@8192
	D=A
	@R0
	M=D			//r0 = 8192
	
	@SCREEN
	D=A
	@R1
	M=D			//r1= @screen
	
(LOOP)	
	@R0
	D=M;
	@BEGIN
	D;JEQ		//r0==0 -> BEGIN
	
	
	@KBD
	D=M
	
	@PRESSED
	D;JNE
	
	D=0
	@ENDIF
	0;JEQ
(PRESSED)
	D=-1
(ENDIF)

	@R1
	A=M
	M=D
	
	@R1
	M=M+1
	
	@R0
	M=M-1
	
	@R0
	D=M;

	@LOOP
	0;JEQ
