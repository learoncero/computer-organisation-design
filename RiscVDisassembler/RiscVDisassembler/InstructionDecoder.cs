namespace RiscVDisassembler {
    internal record DecodedInstruction {
        public uint Opcode { get; init; }
        public uint Rd { get; init; }
        public uint Rs1 { get; init; }
        public uint Rs2 { get; init; }
        public uint Funct3 { get; init; }
        public uint Funct7 { get; init; }
        public int Immediate { get; init; }
        public bool IsValid { get; init; } = true;

        public static DecodedInstruction Decode(uint instruction) {
            if ((instruction & InstructionConstants.Base32IntegerMask) != 0x03) {
                return new DecodedInstruction { IsValid = false };
            }

            return new DecodedInstruction {
                Opcode = (instruction >> InstructionConstants.OpcodeShift) & InstructionConstants.OpcodeMask,
                Rd = (instruction >> InstructionConstants.RdShift) & InstructionConstants.RdMask,
                Rs1 = (instruction >> InstructionConstants.Rs1Shift) & InstructionConstants.Rs1Mask,
                Rs2 = (instruction >> InstructionConstants.Rs2Shift) & InstructionConstants.Rs2Mask,
                Funct3 = (instruction >> InstructionConstants.Funct3Shift) & InstructionConstants.Funct3Mask,
                Funct7 = (instruction >> InstructionConstants.Funct7Shift) & InstructionConstants.Funct7Mask,
                Immediate = 0, // Will be computed based on instruction type
                IsValid = true
            };
        }
    }

    internal static class InstructionDecoder {
        public static string Disassemble(uint instruction) {
            var decoded = DecodedInstruction.Decode(instruction);

            if (!decoded.IsValid) {
                return $"invalid instruction (not 32-bit base format)";
            }

            return decoded.Opcode switch {
                InstructionConstants.OpcodeRType => DisassembleRType(decoded),
                InstructionConstants.OpcodeITypeArith => DisassembleITypeArithmetic(instruction, decoded),
                InstructionConstants.OpcodeITypeLoad => DisassembleITypeLoad(instruction, decoded),
                InstructionConstants.OpcodeSType => DisassembleSType(instruction, decoded),
                InstructionConstants.OpcodeJalr => DisassembleJalr(instruction, decoded),
                InstructionConstants.OpcodeJal => DisassembleJal(instruction, decoded),
                InstructionConstants.OpcodeLui => DisassembleUTypeLUI(instruction, decoded),
                InstructionConstants.OpcodeAuipc => DisassembleUTypeAUIPC(instruction, decoded),
                InstructionConstants.OpcodeBType => DisassembleBType(instruction, decoded),
                _ => $"unknown opcode 0x{decoded.Opcode:X2}"
            };
        }

        private static string DisassembleRType(DecodedInstruction decoded) {
            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);
            string rs2 = RegisterSet.GetRegisterName(decoded.Rs2);

            return (decoded.Funct3, decoded.Funct7) switch {
                (0, 0) => $"add {rd}, {rs1}, {rs2}",
                (0, 32) => $"sub {rd}, {rs1}, {rs2}",
                (1, 0) => $"sll {rd}, {rs1}, {rs2}",
                (2, 0) => $"slt {rd}, {rs1}, {rs2}",
                (4, 0) => $"xor {rd}, {rs1}, {rs2}",
                (5, 0) => $"srl {rd}, {rs1}, {rs2}",
                (5, 32) => $"sra {rd}, {rs1}, {rs2}",
                (6, 0) => $"or {rd}, {rs1}, {rs2}",
                (7, 0) => $"and {rd}, {rs1}, {rs2}",
                // RV32M extension
                (0, 1) => $"mul {rd}, {rs1}, {rs2}",
                (1, 1) => $"mulh {rd}, {rs1}, {rs2}",
                (2, 1) => $"mulhsu {rd}, {rs1}, {rs2}",
                (3, 1) => $"mulhu {rd}, {rs1}, {rs2}",
                (4, 1) => $"div {rd}, {rs1}, {rs2}",
                (5, 1) => $"divu {rd}, {rs1}, {rs2}",
                (6, 1) => $"rem {rd}, {rs1}, {rs2}",
                (7, 1) => $"remu {rd}, {rs1}, {rs2}",
                _ => $"unknown R-type funct3={decoded.Funct3}, funct7={decoded.Funct7}"
            };
        }

        private static string DisassembleITypeArithmetic(uint instruction, DecodedInstruction decoded) {
            int imm = BitHelpers.SignExtend12((int)((instruction >> InstructionConstants.Imm12Shift) & InstructionConstants.Imm12Mask));

            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);

            return decoded.Funct3 switch {
                0 => $"addi {rd}, {rs1}, {imm}",
                2 => $"slti {rd}, {rs1}, {imm}",
                4 => $"xori {rd}, {rs1}, {imm}",
                6 => $"ori {rd}, {rs1}, {imm}",
                7 => $"andi {rd}, {rs1}, {imm}",
                1 => $"slli {rd}, {rs1}, {imm & InstructionConstants.ShiftMask}",
                5 when decoded.Funct7 == 0 => $"srli {rd}, {rs1}, {imm & InstructionConstants.ShiftMask}",
                5 when decoded.Funct7 == 32 => $"srai {rd}, {rs1}, {imm & InstructionConstants.ShiftMask}",
                _ => $"unknown I-type Arithmetic funct3={decoded.Funct3}"
            };
        }

        private static string DisassembleITypeLoad(uint instruction, DecodedInstruction decoded) {
            int imm = BitHelpers.SignExtend12((int)((instruction >> InstructionConstants.Imm12Shift) & InstructionConstants.Imm12Mask));

            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);

            return decoded.Funct3 switch {
                0 => $"lb {rd}, {imm}({rs1})",
                1 => $"lh {rd}, {imm}({rs1})",
                2 => $"lw {rd}, {imm}({rs1})",
                3 => $"lbu {rd}, {imm}({rs1})",
                4 => $"lhu {rd}, {imm}({rs1})",
                _ => $"unknown I-type Load funct3={decoded.Funct3}"
            };
        }

        private static string DisassembleSType(uint instruction, DecodedInstruction decoded) {
            uint imm4_0 = (instruction >> InstructionConstants.RdShift) & InstructionConstants.RdMask;
            uint imm11_5 = decoded.Funct7;
            int imm = BitHelpers.SignExtend12((int)((imm11_5 << 5) | imm4_0));

            string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);
            string rs2 = RegisterSet.GetRegisterName(decoded.Rs2);

            return decoded.Funct3 switch {
                0 => $"sb {rs2}, {imm}({rs1})",
                1 => $"sh {rs2}, {imm}({rs1})",
                2 => $"sw {rs2}, {imm}({rs1})",
                _ => $"unknown S-type funct3={decoded.Funct3}"
            };
        }

        private static string DisassembleJalr(uint instruction, DecodedInstruction decoded) {
            uint imm = (instruction >> InstructionConstants.Imm12Shift) & InstructionConstants.Imm12Mask;

            // Check special-case RET (jalr x0, 0(x1))
            if (decoded.Funct3 == 0 && decoded.Rd == 0 && decoded.Rs1 == 1 && imm == 0) {
                return "ret";
            }

            if (decoded.Funct3 == 0) {
                string rd = RegisterSet.GetRegisterName(decoded.Rd);
                string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);
                return $"jalr {rd}, {imm}({rs1})";
            }

            return "unknown JALR";
        }

        private static string DisassembleJal(uint instruction, DecodedInstruction decoded) {
            int imm = ((int)((instruction >> 31) & 0x1) << 20) |   // bit 20
                      ((int)((instruction >> 21) & 0x3FF) << 1) |  // bits 10:1
                      ((int)((instruction >> 20) & 0x1) << 11) |   // bit 11
                      ((int)((instruction >> 12) & 0xFF) << 12);   // bits 19:12
            imm = BitHelpers.SignExtend21(imm);

            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            return $"jal {rd}, {imm}";
        }

        private static string DisassembleUTypeLUI(uint instruction, DecodedInstruction decoded) {
            int imm = (int)(instruction & InstructionConstants.Imm20Mask);
            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            return $"lui {rd}, 0x{imm:X}";
        }

        private static string DisassembleUTypeAUIPC(uint instruction, DecodedInstruction decoded) {
            int imm = (int)(instruction & InstructionConstants.Imm20Mask);
            string rd = RegisterSet.GetRegisterName(decoded.Rd);
            return $"auipc {rd}, 0x{imm:X}";
        }

        private static string DisassembleBType(uint instruction, DecodedInstruction decoded) {
            int imm = ((int)((instruction >> 31) & 0x1) << 12) |   // bit 12 (sign)
                      ((int)((instruction >> 25) & 0x3F) << 5) |    // bits 10:5
                      ((int)((instruction >> 8) & 0xF) << 1) |      // bits 4:1
                      ((int)((instruction >> 7) & 0x1) << 11);      // bit 11

            // Sign-extend 13-bit immediate (bits 12:0)
            if ((imm & 0x1000) != 0)
                imm |= unchecked((int)0xFFFFE000);

            string rs1 = RegisterSet.GetRegisterName(decoded.Rs1);
            string rs2 = RegisterSet.GetRegisterName(decoded.Rs2);

            return decoded.Funct3 switch {
                0 => $"beq {rs1}, {rs2}, {imm}",
                1 => $"bne {rs1}, {rs2}, {imm}",
                4 => $"blt {rs1}, {rs2}, {imm}",
                5 => $"bge {rs1}, {rs2}, {imm}",
                6 => $"bltu {rs1}, {rs2}, {imm}",
                7 => $"bgeu {rs1}, {rs2}, {imm}",
                _ => $"unknown B-type funct3={decoded.Funct3}"
            };
        }
    }
}