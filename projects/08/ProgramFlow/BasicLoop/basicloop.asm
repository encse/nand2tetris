//SP=256
	@256
	D=A
	@0
	M=D
//push __lbl0
	@__lbl0
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//push LCL
	@LCL
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push ARG
	@ARG
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push THIS
	@THIS
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push THAT
	@THAT
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//ARG=SP-n-5
	@SP
	D=M
	@5
	D=D-A
	@ARG
	M=D
//LCL=SP
	@SP
	D=M
	@LCL
	M=D
	@Sys.init
	D;JMP
(__lbl0)
//push constant 0
	@0
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//pop local 0
	@LCL
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
//label LOOP_START
(LOOP_START)
//push argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push local 0
	@LCL
	D=M
	@0
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
//pop local 0
	@LCL
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
//push argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//push constant 1
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
//pop argument 0
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
//push argument 0
	@ARG
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//if-goto LOOP_START
	@SP
	M=M-1
	A=M
	D=M
	@LOOP_START
	D;JNE
//push local 0
	@LCL
	D=M
	@0
	A=D+A
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
