//Push Constant 111
	@111
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//Push Constant 333
	@333
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//Push Constant 888
	@888
	D=A
	@SP
	M=M+1
	A=M-1
	M=D
//Pop Static 8
	@statictest.vm.8
	D=A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//Pop Static 3
	@statictest.vm.3
	D=A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//Pop Static 1
	@statictest.vm.1
	D=A
	@R13
	M=D
	@SP
	M=M-1
	A=M
	D=M
	@R13
	A=M
	M=D
//Push Static 3
	@statictest.vm.3
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//Push Static 1
	@statictest.vm.1
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//Sub
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
//Push Static 8
	@statictest.vm.8
	D=M
	@SP
	M=M+1
	A=M-1
	M=D
//Add
	@SP
	A=M-1
	D=M
	A=A-1
	M=D+M
	@SP
	M=M-1
