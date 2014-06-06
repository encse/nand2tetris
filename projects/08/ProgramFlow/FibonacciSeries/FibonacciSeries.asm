//push Argument 1
	@ARG
	D=M
	@1
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//pop Pointer 1
	@THIS
	D=A
	@1
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//push Constant 0
	@0
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//pop That 0
	@THAT
	D=M
	@0
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//push Constant 1
	@1
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//pop That 1
	@THAT
	D=M
	@1
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//push Argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push Constant 2
	@2
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//sub
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
//pop Argument 0
	@ARG
	D=M
	@0
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//label MAIN_LOOP_START
(MAIN_LOOP_START)
//push Argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//if-goto COMPUTE_ELEMENT
	@SP
	M=M-1
	A=M
	D=M
	@COMPUTE_ELEMENT
	D;JNE
//goto END_PROGRAM
	@END_PROGRAM
	D;JMP
//label COMPUTE_ELEMENT
(COMPUTE_ELEMENT)
//push That 0
	@THAT
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push That 1
	@THAT
	D=M
	@1
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//add
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
//pop That 2
	@THAT
	D=M
	@2
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//push Pointer 1
	@THIS
	D=A
	@1
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push Constant 1
	@1
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//add
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
//pop Pointer 1
	@THIS
	D=A
	@1
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//push Argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push Constant 1
	@1
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//sub
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
//pop Argument 0
	@ARG
	D=M
	@0
	D=D+A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//goto MAIN_LOOP_START
	@MAIN_LOOP_START
	D;JMP
//label END_PROGRAM
(END_PROGRAM)
