	@256
	D=A
	@0
	M=D
	@0
	D=A
	@R14
	M=D
	@Sys.init
	D=A
	@R15
	M=D
	@__return_3
	D=A
	@__funcall_0
	D;JMP
(__return_3)
(__end_2)
	@__end_2
	0;JMP
(__funcall_0)
		@SP
		AM=M+1
		A=A-1
		M=D
		@LCL
		D=M
		@SP
		AM=M+1
		A=A-1
		M=D
		@ARG
		D=M
		@SP
		AM=M+1
		A=A-1
		M=D
		@THIS
		D=M
		@SP
		AM=M+1
		A=A-1
		M=D
		@THAT
		D=M
		@SP
		AM=M+1
		A=A-1
		M=D
		@SP
		D=M
		@R14
		D=D-M
		@5
		D=D-A
		@ARG
		M=D
		@SP
		D=M
		@LCL
		M=D
		@R15
		A=M
		D;JMP
(__return_1)
		@LCL
		D=M
		@R13
		M=D
		@5
		A=D-A
		D=M
		@R14
		M=D
		@SP
		A=M-1
		D=M
		@ARG
		A=M
		M=D
		@ARG
		D=M+1
		@SP
		M=D
		@R13
		AM=M-1
		D=M
		@THAT
		M=D
		@R13
		AM=M-1
		D=M
		@THIS
		M=D
		@R13
		AM=M-1
		D=M
		@ARG
		M=D
		@R13
		AM=M-1
		D=M
		@LCL
		M=D
		@R14
		A=M
		D;JMP
	@111
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@333
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@888
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@SP
	AM=M-1
	D=M
	@StaticTest.8
	M=D
	@SP
	AM=M-1
	D=M
	@StaticTest.3
	M=D
	@SP
	AM=M-1
	D=M
	@StaticTest.1
	M=D
	@StaticTest.3
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
	@StaticTest.1
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
	@StaticTest.8
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
