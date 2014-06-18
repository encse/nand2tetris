//push constant 0
	@SP
	AM=M+1
	A=A-1
	M=0
//pop local 0
	@LCL
	D=M
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
//label LOOP_START
($LOOP_START)
//push argument 0
	@ARG
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//push local 0
	@LCL
	D=M
	A=M
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
//pop local 0
	@LCL
	D=M
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
//push argument 0
	@ARG
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
//if-goto LOOP_START
	@SP
	AM=M-1
	D=M
	@LOOP_START
	D;JNE
//push local 0
	@LCL
	D=M
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
