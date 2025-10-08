using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RiscVDisassembler
{
    internal class RiscVDisassembler
    {
        private static string[] registers = {
            "x0","x1","x2","x3","x4","x5","x6","x7",
            "x8","x9","x10","x11","x12","x13","x14","x15",
            "x16","x17","x18","x19","x20","x21","x22","x23",
            "x24","x25","x26","x27","x28","x29","x30","x31", "pc"
        };

        private static void Main()
        {
            uint[] program = {
                0xFE010113, 0x00812E23, 0x02010413, 0xFEA42623,
                0xFEC42783, 0x02F787B3, 0x00078513, 0x01C12403,
                0x02010113, 0x00008067,
            };

            foreach (uint instruction in program) {
                string disassembledInstruction = Disassemble(instruction);
                Console.WriteLine($"0x{instruction:X8} : {disassembledInstruction}");

            }
        }

        private static string Disassemble(uint instruction) {
            uint opcode = instruction & 0x7F; // 0x7F = extract the 7 least significant bits by masking it with 0111 1111

            switch (opcode) {
                case 0x33:
                    return DisassembleRType(instruction);
                case 0xB3:
                    return DisassembleRType(instruction);
                case 0x13:
                    return DisassembleITypeArithmetic(instruction);
                case 0x23:
                    return DisassembleSType(instruction);
                case 0x03:
                    return DisassembleITypeLoad(instruction);
                case 0x67: 
                    return DisassembleJalr(instruction);
                default:
                    return "unknown opcode";
            }
            
        }

        private static string DisassembleJalr(uint instruction) {
            uint rd = ((instruction >> 7) & 0x1F);
            uint funct3 = ((instruction >> 12) & 0x7);
            uint rs1 = ((instruction >> 15) & 0x1F);
            uint imm = ((instruction >> 20) & 0xFFF);

            // Check special-case RET
            if (funct3 == 0 && rd == 0 && rs1 == 1 && imm == 0)
                return "ret";

            // Otherwise, normal JALR
            if (funct3 == 0)
                return $"jalr {registers[rd]}, {imm}({registers[rs1]})";

            return "unknown JALR";
        }

        private static string DisassembleRType(uint instruction) {
            uint rd = ((instruction >> 7) & 0x1F);
            uint funct3 = ((instruction >> 12) & 0x7);
            uint rs1 = ((instruction >> 15) & 0x1F);
            uint rs2 = ((instruction >> 20) & 0x1F);
            uint funct7 = ((instruction >> 25) & 0x7F);

            if ((funct3 == 0) && (funct7 == 0)) {
                return $"add {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 2) && (funct7 == 0)) {
                return $"slt {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 7) && (funct7 == 0)) {
                return $"and {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 6) && (funct7 == 0)) {
                return $"or {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 4) && (funct7 == 0)) {
                return $"xor {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 1) && (funct7 == 0)) {
                return $"sll {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 5) && (funct7 == 0)) {
                return $"srl {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }
            
            if ((funct3 == 0) && (funct7 == 32)) {
                return $"sub {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 5) && (funct7 == 32)) {
                return $"sra {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            // RV32M Standard Extension

            if ((funct3 == 0) && (funct7 == 1)) {
                return $"mul {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 1) && (funct7 == 1)) {
                return $"mulh {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 2) && (funct7 == 1)) {
                return $"mulhsu {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 3) && (funct7 == 1)) {
                return $"mulhu {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 4) && (funct7 == 1)) {
                return $"div {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 5) && (funct7 == 1)) {
                return $"divu {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 6) && (funct7 == 1)) {
                return $"rem {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            if ((funct3 == 7) && (funct7 == 1)) {
                return $"remu {registers[rd]}, {registers[rs1]}, {registers[rs2]}";
            }

            return "unknown R-type";
        }

        private static string DisassembleITypeArithmetic(uint instruction) {
            uint rd = ((instruction >> 7) & 0x1F);
            uint funct3 = ((instruction >> 12) & 0x7);
            uint rs1 = ((instruction >> 15) & 0x1F);
            uint imm = ((instruction >> 20) & 0xFFF);

            if (funct3 == 0) {
                return $"addi {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 2) {
                return $"slti {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 7) {
                return $"andi {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 6) {
                return $"ori {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 4) {
                return $"xori {registers[rd]}, {registers[rs1]}, {imm}";
            }

            return "unknown I-type Arithmetic";
        }

        private static string DisassembleSType(uint instruction) {
            uint imm4_0 = ((instruction >> 7) & 0x1F);
            uint funct3 = ((instruction >> 12) & 0x7);
            uint rs1 = ((instruction >> 15) & 0x1F);
            uint rs2 = ((instruction >> 20) & 0x1F);
            uint imm11_5 = ((instruction >> 25) & 0x7F);

            uint imm = (imm11_5 << 5) | imm4_0;

            if (funct3 == 0) {
                return $"sb {registers[rs2]}, {imm}({registers[rs1]})";
            }

            if (funct3 == 1) {
                return $"sh {registers[rs2]}, {imm}({registers[rs1]})";
            }

            if (funct3 == 2) {
                return $"sw {registers[rs2]}, {imm}({registers[rs1]})";

            }

            return "unknown S-type";
        }

        private static string DisassembleITypeLoad(uint instruction) {
            uint rd = ((instruction >> 7) & 0x1F);
            uint funct3 = ((instruction >> 12) & 0x7);
            uint rs1 = ((instruction >> 15) & 0x1F);
            uint imm = ((instruction >> 20) & 0xFFF);

            if (funct3 == 0) {
                return $"lb {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 1) {
                return $"lh {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 2) {
                return $"lw {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 3) {
                return $"lbu {registers[rd]}, {registers[rs1]}, {imm}";
            }

            if (funct3 == 4) {
                return $"lhu {registers[rd]}, {registers[rs1]}, {imm}";
            }

            return "unknown I-Type Load";
        }
    }   
}
