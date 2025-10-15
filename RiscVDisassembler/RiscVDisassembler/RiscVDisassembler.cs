//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Security.Cryptography;

//namespace RiscVDisassembler
//{
//    internal class RiscVDisassembler
//    {
//        private static string[] Registers = {
//            "x0","x1","x2","x3","x4","x5","x6","x7",
//            "x8","x9","x10","x11","x12","x13","x14","x15",
//            "x16","x17","x18","x19","x20","x21","x22","x23",
//            "x24","x25","x26","x27","x28","x29","x30","x31", "pc"
//        };

//        private static void Main()
//        {
//            string filePath = "Resources\\program.txt";
//            List<uint> program = new List<uint>();

//            foreach (string line in File.ReadAllLines(filePath)) {
//                // skip empty lines
//                if (string.IsNullOrWhiteSpace(line))
//                    continue;

//                // split by commas
//                string[] parts = line.Split(',');

//                foreach (string part in parts) {
//                    string trimmed = part.Trim();

//                    if (string.IsNullOrEmpty(trimmed))
//                        continue;

//                    // parse hex to uint
//                    uint instruction = Convert.ToUInt32(trimmed, 16);
//                    program.Add(instruction);
//                }
//            }

//            foreach (uint instruction in program) {
//                string disassembledinstruction = Disassemble(instruction);
//                Console.WriteLine($"0x{instruction:x8} : {disassembledinstruction}");

//            }
//        }

//        private static string Disassemble(uint instruction) {
//            uint opcode = instruction & 0x7F; // 0x7F = extract the 7 least significant bits by masking it with 0111 1111

//            return opcode switch {
//                0x33 => DisassembleRType(instruction),
//                0x13 => DisassembleITypeArithmetic(instruction),
//                0x03 => DisassembleITypeLoad(instruction),
//                0x23 => DisassembleSType(instruction),
//                0x67 => DisassembleJalr(instruction),
//                0x6F => DisassembleJal(instruction),
//                0x37 => DisassembleUTypeLUI(instruction),
//                0x17 => DisassembleUTypeAUIPC(instruction),
//                0x63 => DisassembleBType(instruction),
//                _ => $"unknown opcode 0x{opcode:X2}"
//            };
//        }

//        private static string DisassembleRType(uint instruction) {
//            uint rd = ((instruction >> 7) & 0x1F);
//            uint funct3 = ((instruction >> 12) & 0x7);
//            uint rs1 = ((instruction >> 15) & 0x1F);
//            uint rs2 = ((instruction >> 20) & 0x1F);
//            uint funct7 = ((instruction >> 25) & 0x7F);

//            return (funct3, funct7) switch {
//                (0, 0) => $"add {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (0, 32) => $"sub {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (1, 0) => $"sll {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (2, 0) => $"slt {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (4, 0) => $"xor {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (5, 0) => $"srl {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (5, 32) => $"sra {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (6, 0) => $"or {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (7, 0) => $"and {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                // RV32M extension
//                (0, 1) => $"mul {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (1, 1) => $"mulh {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (2, 1) => $"mulhsu {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (3, 1) => $"mulhu {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (4, 1) => $"div {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (5, 1) => $"divu {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (6, 1) => $"rem {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                (7, 1) => $"remu {Registers[rd]}, {Registers[rs1]}, {Registers[rs2]}",
//                _ => $"unknown R-type funct3={funct3}, funct7={funct7}"
//            };
//        }

//        private static string DisassembleITypeArithmetic(uint instruction) {
//            uint rd = ((instruction >> 7) & 0x1F);
//            uint funct3 = ((instruction >> 12) & 0x7);
//            uint rs1 = ((instruction >> 15) & 0x1F);
//            int imm = SignExtend12((int)((instruction >> 20) & 0xFFF));

//            return funct3 switch {
//                0 => $"addi {Registers[rd]}, {Registers[rs1]}, {imm}",
//                2 => $"slti {Registers[rd]}, {Registers[rs1]}, {imm}",
//                4 => $"xori {Registers[rd]}, {Registers[rs1]}, {imm}",
//                6 => $"ori {Registers[rd]}, {Registers[rs1]}, {imm}",
//                7 => $"andi {Registers[rd]}, {Registers[rs1]}, {imm}",
//                1 => $"slli {Registers[rd]}, {Registers[rs1]}, {imm & 0x1F}", // shift amounts only lower 5 bits
//                5 when ((instruction >> 25) & 0x7F) == 0 => $"srli {Registers[rd]}, {Registers[rs1]}, {imm & 0x1F}",
//                5 when ((instruction >> 25) & 0x7F) == 32 => $"srai {Registers[rd]}, {Registers[rs1]}, {imm & 0x1F}",
//                _ => $"unknown I-type Arithmetic funct3={funct3}"
//            };
//        }

//        private static string DisassembleITypeLoad(uint instruction) {
//            uint rd = ((instruction >> 7) & 0x1F);
//            uint funct3 = ((instruction >> 12) & 0x7);
//            uint rs1 = ((instruction >> 15) & 0x1F);
//            int imm = SignExtend12((int)((instruction >> 20) & 0xFFF));

//            return funct3 switch {
//                0 => $"lb {Registers[rd]}, {imm}({Registers[rs1]})",
//                1 => $"lh {Registers[rd]}, {imm}({Registers[rs1]})",
//                2 => $"lw {Registers[rd]}, {imm}({Registers[rs1]})",
//                3 => $"lbu {Registers[rd]}, {imm}({Registers[rs1]})",
//                4 => $"lhu {Registers[rd]}, {imm}({Registers[rs1]})",
//                _ => $"unknown I-type Load funct3={funct3}"
//            };
//        }

//        private static string DisassembleSType(uint instruction) {
//            uint imm4_0 = ((instruction >> 7) & 0x1F);
//            uint funct3 = ((instruction >> 12) & 0x7);
//            uint rs1 = ((instruction >> 15) & 0x1F);
//            uint rs2 = ((instruction >> 20) & 0x1F);
//            uint imm11_5 = ((instruction >> 25) & 0x7F);
//            int imm = SignExtend12((int)((imm11_5 << 5) | imm4_0));

//            return funct3 switch {
//                0 => $"sb {Registers[rs2]}, {imm}({Registers[rs1]})",
//                1 => $"sh {Registers[rs2]}, {imm}({Registers[rs1]})",
//                2 => $"sw {Registers[rs2]}, {imm}({Registers[rs1]})",
//                _ => $"unknown S-type funct3={funct3}"
//            };
//        }

//        private static string DisassembleJalr(uint instruction) {
//            uint rd = ((instruction >> 7) & 0x1F);
//            uint funct3 = ((instruction >> 12) & 0x7);
//            uint rs1 = ((instruction >> 15) & 0x1F);
//            uint imm = ((instruction >> 20) & 0xFFF);

//            // Check special-case RET
//            if (funct3 == 0 && rd == 0 && rs1 == 1 && imm == 0) {
//                return "ret";
//            }

//            // Otherwise, normal JALR
//            if (funct3 == 0) {
//                return $"jalr {Registers[rd]}, {imm}({Registers[rs1]})";
//            } else {
//                return "unknown JALR";
//            }
//        }

//        private static string DisassembleJal(uint instr) {
//            uint rd = (instr >> 7) & 0x1F;

//            int imm = ((int)((instr >> 31) & 0x1) << 20) |   // bit 20
//                      ((int)((instr >> 21) & 0x3FF) << 1) |  // bits 10:1
//                      ((int)((instr >> 20) & 0x1) << 11) |   // bit 11
//                      ((int)((instr >> 12) & 0xFF) << 12);   // bits 19:12
//            imm = SignExtend21(imm);

//            return $"jal {Registers[rd]}, {imm}";
//        }

//        private static string DisassembleUTypeLUI(uint instruction) {
//            uint rd = ((instruction >> 7) & 0x1F);
//            int imm = (int)(instruction & 0xFFFFF000);

//            return $"lui {Registers[rd]}, 0x{imm:X}";
//        }

//        private static string DisassembleUTypeAUIPC(uint instr) {
//            uint rd = (instr >> 7) & 0x1F;
//            int imm = (int)(instr & 0xFFFFF000);

//            return $"auipc {Registers[rd]}, 0x{imm:X}";
//        }

//        private static string DisassembleBType(uint instruction) {
//            uint funct3 = (instruction >> 12) & 0x7;
//            uint rs1 = (instruction >> 15) & 0x1F;
//            uint rs2 = (instruction >> 20) & 0x1F;

//            // Construct 12-bit immediate from B-type encoding
//            int imm = ((int)((instruction >> 31) & 0x1) << 12) |   // bit 12 (sign)
//                      ((int)((instruction >> 25) & 0x3F) << 5) |    // bits 10:5
//                      ((int)((instruction >> 8) & 0xF) << 1) |      // bits 4:1
//                      ((int)((instruction >> 7) & 0x1) << 11);      // bit 11

//            // Sign-extend 13-bit immediate (bits 12:0)
//            if ((imm & 0x1000) != 0)
//                imm |= unchecked((int)0xFFFFE000);

//            return funct3 switch {
//                0 => $"beq {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                1 => $"bne {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                4 => $"blt {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                5 => $"bge {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                6 => $"bltu {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                7 => $"bgeu {Registers[rs1]}, {Registers[rs2]}, {imm}",
//                _ => $"unknown B-type funct3={funct3}"
//            };
//        }

//        // helpers for sign-extension
//        private static int SignExtend12(int imm) => (imm & 0x800) != 0 ? imm | unchecked((int)0xFFFFF000) : imm;
//        private static int SignExtend21(int imm) => (imm & 0x100000) != 0 ? imm | unchecked((int)0xFFE00000) : imm;
//    }
//}
