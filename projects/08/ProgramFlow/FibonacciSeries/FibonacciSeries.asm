//push argument 1
	@ARG
	A=M+1
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//pop pointer 1
	@THIS
	D=A
	@1
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//push constant 0
	@SP
	AM=M+1
	A=A-1
	M=0
//pop that 0
	@THAT
	D=M
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//push constant 1
	@SP
	AM=M+1
	A=A-1
	M=1
//pop that 1
	@THAT
	D=M
	@1
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//push argument 0
	@ARG
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//push constant 2
	@2
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
//sub
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
//pop argument 0
	@ARG
	D=M
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//label MAIN_LOOP_START
($MAIN_LOOP_START)
//push argument 0
	@ARG
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//if-goto COMPUTE_ELEMENT
	@SP
	AM=M-1
	D=M
	@COMPUTE_ELEMENT
	D;JNE
//goto END_PROGRAM
	@$END_PROGRAM
	D;JMP
//label COMPUTE_ELEMENT
($COMPUTE_ELEMENT)
//push that 0
	@THAT
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//push that 1
	@THAT
	A=M+1
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//add
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
//pop that 2
	@THAT
	D=M
	@2
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//push pointer 1
	@THIS
	A=A+1
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//push constant 1
	@SP
	AM=M+1
	A=A-1
	M=1
//add
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
//pop pointer 1
	@THIS
	D=A
	@1
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//push argument 0
	@ARG
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//push constant 1
	@SP
	AM=M+1
	A=A-1
	M=1
//sub
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
//pop argument 0
	@ARG
	D=M
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//goto MAIN_LOOP_START
	@$MAIN_LOOP_START
	D;JMP
//label END_PROGRAM
($END_PROGRAM)
