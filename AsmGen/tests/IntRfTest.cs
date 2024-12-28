﻿using System.Text;

namespace AsmGen
{
    public class IntRfTest : UarchTest
    {
        private bool initialDependentBranch;
        public IntRfTest(int low, int high, int step, bool initialDependentBranch)
        {
            this.Counts = UarchTestHelpers.GenerateCountArray(low, high, step);
            this.Prefix = "intrf" + (initialDependentBranch ? "db" : string.Empty);
            this.Description = "Integer Register File" + (initialDependentBranch ? ", preceded by dependent branch" : string.Empty);
            this.FunctionDefinitionParameters = "uint64_t iterations, int *arr";
            this.GetFunctionCallParameters = "structIterations, A";
            this.DivideTimeByCount = false;
            this.initialDependentBranch = initialDependentBranch;
        }

        public override bool SupportsIsa(IUarchTest.ISA isa)
        {
            if (this.initialDependentBranch && isa != IUarchTest.ISA.aarch64) return false;
            if (isa == IUarchTest.ISA.amd64) return true;
            if (isa == IUarchTest.ISA.aarch64) return true;
            if (isa == IUarchTest.ISA.mips64) return true;
            if (isa == IUarchTest.ISA.riscv) return true;
            return false;
        }

        public override void GenerateAsm(StringBuilder sb, IUarchTest.ISA isa)
        {
            if (isa == IUarchTest.ISA.amd64)
            {
                string[] unrolledAdds = new string[4];
                unrolledAdds[0] = "  add %r11, %r15";
                unrolledAdds[1] = "  add %r11, %r14";
                unrolledAdds[2] = "  add %r11, %r13";
                unrolledAdds[3] = "  add %r11, %r12";
                UarchTestHelpers.GenerateX86AsmStructureTestFuncs(sb, this.Counts, this.Prefix, unrolledAdds, unrolledAdds, includePtrChasingLoads: true);
            }
            else if (isa == IUarchTest.ISA.aarch64)
            {
                string postLoadInstrs = this.initialDependentBranch ? UarchTestHelpers.GetArmDependentBranch(this.Prefix) : null;
                string[] unrolledAdds = new string[4];
                unrolledAdds[0] = "  add x15, x15, x11";
                unrolledAdds[1] = "  add x14, x14, x11";
                unrolledAdds[2] = "  add x13, x13, x11";
                unrolledAdds[3] = "  add x12, x12, x11";
                UarchTestHelpers.GenerateArmAsmStructureTestFuncs(
                    sb, this.Counts, this.Prefix, unrolledAdds, unrolledAdds, includePtrChasingLoads: true, postLoadInstrs1: postLoadInstrs, postLoadInstrs2: postLoadInstrs);
                if (this.initialDependentBranch) sb.AppendLine(UarchTestHelpers.GetArmDependentBranchTarget(this.Prefix));
            }
            else if (isa == IUarchTest.ISA.mips64)
            {
                string[] unrolledAdds = new string[4];
                unrolledAdds[0] = "  add.d $r15, $r15, $r14";
                unrolledAdds[1] = "  add.d $r16, $r16, $r14";
                unrolledAdds[2] = "  add.d $r17, $r17, $r14";
                unrolledAdds[3] = "  add.d $r18, $r18, $r14";
                UarchTestHelpers.GenerateMipsAsmStructureTestFuncs(sb, this.Counts, this.Prefix, unrolledAdds, unrolledAdds, includePtrChasingLoads: true);
            }
            else if (isa == IUarchTest.ISA.riscv)
            {
                string postLoadInstrs = this.initialDependentBranch ? UarchTestHelpers.GetRiscvDependentBranch(this.Prefix) : null;
                string[] unrolledAdds = new string[4];
                unrolledAdds[0] = "  add x28, x28, x29";
                unrolledAdds[1] = "  add x30, x30, x29";
                unrolledAdds[2] = "  add x31, x31, x29";
                unrolledAdds[3] = "  add x18, x18, x29";
                UarchTestHelpers.GenerateRiscvAsmStructureTestFuncs(sb, this.Counts, this.Prefix, unrolledAdds, unrolledAdds, includePtrChasingLoads: true, postLoadInstrs1: postLoadInstrs, postLoadInstrs2: postLoadInstrs);
                if (this.initialDependentBranch) sb.AppendLine(UarchTestHelpers.GetRiscvDependentBranchTarget(this.Prefix));
            }
        }
    }
}
