	@256
	D=A
	@0
	M=D
	@R14
	M=0
	@Sys.init
	D=A
	@R15
	M=D
	@__after_call_6
	D=A
	@__funcall_0
	D;JMP
(__after_call_6)
(__end_5)
	@__end_5
	0;JMP
(__eq_2)
	@R14
	M=D
	@SP
	A=M-1
	D=M
	A=A-1
	D=M-D
	@__lbl_7
	D;JEQ
	@__lbl_8
	D=0;JEQ
(__lbl_7)
	D=-1
(__lbl_8)
	@SP
	M=M-1
	A=M-1
	M=D
	@R14
	A=M
	D;JMP
(__gt_3)
	@R14
	M=D
	@SP
	A=M-1
	D=M
	A=A-1
	D=M-D
	@__lbl_9
	D;JGT
	@__lbl_10
	D=0;JEQ
(__lbl_9)
	D=-1
(__lbl_10)
	@SP
	M=M-1
	A=M-1
	M=D
	@R14
	A=M
	D;JMP
(__lt_4)
	@R14
	M=D
	@SP
	A=M-1
	D=M
	A=A-1
	D=M-D
	@__lbl_11
	D;JLT
	@__lbl_12
	D=0;JEQ
(__lbl_11)
	D=-1
(__lbl_12)
	@SP
	M=M-1
	A=M-1
	M=D
	@R14
	A=M
	D;JMP
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
	@10
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@SP
	AM=M-1
	D=M
	@LCL
	A=M
	M=D
	@21
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@22
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@ARG
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
	@SP
	AM=M-1
	D=M
	@ARG
	A=M+1
	M=D
	@36
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@THIS
	D=M
	@6
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
	@42
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@45
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@THAT
	D=M
	@5
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
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
	@510
	D=A
	@SP
	AM=M+1
	A=A-1
	M=D
	@R5
	D=A
	@6
	D=D+A
	@R13
	M=D
	@SP
	AM=M-1
	D=M
	@R13
	A=M
	M=D
	@LCL
	A=M
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
	@THAT
	D=M
	@5
	A=D+A
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
	@ARG
	A=M+1
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
	@THIS
	D=M
	@6
	A=D+A
	D=M
	@SP
	AM=M+1
	A=A-1
	M=D
	@THIS
	D=M
	@6
	A=D+A
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
	@SP
	A=M-1
	D=M
	A=A-1
	M=M-D
	@SP
	M=M-1
	@R5
	D=A
	@6
	A=D+A
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
