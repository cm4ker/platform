using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SqlPlusDbSync.Data.Model
{
    public class Hdr
    {
        public int? SUSR_ID { get; set; }

        public short Skd_UniCode { get; set; }

        public int Prm_UniCode { get; set; }

        public int? Hdr_NumPart { get; set; }

        public short Hdr_Obj2Type { get; set; }

        public int Hdr_Obj2Code { get; set; }

        public int? Hdr_ObjDopCode1 { get; set; }

        public short? Hdr_ObjDopCode2 { get; set; }

        public short? Hdr_ObjDopCode3 { get; set; }

        public short Hdr_OperCode { get; set; }

        public short? Hdr_Oper2Code { get; set; }

        public string Hdr_NumDcm { get; set; }

        public DateTime Hdr_Date { get; set; }

        public DateTime Hdr_IntDate { get; set; }

        public decimal? Hdr_WeightAll { get; set; }

        public int? Hdr_QntBoxAll { get; set; }

        public bool Hdr_CalcYN { get; set; }

        public short? Hdr_CurrMonth { get; set; }

        public short DcmState { get; set; }

        public decimal? Hdr_ExchPrc1 { get; set; }

        public decimal? Hdr_ExchPrc2 { get; set; }

        public decimal? Hdr_ExchPrc3 { get; set; }

        public decimal? Hdr_ExchPrc4 { get; set; }

        public decimal? Hdr_Sum01 { get; set; }

        public decimal? Hdr_Sum02 { get; set; }

        public decimal? Hdr_Sum03 { get; set; }

        public decimal? Hdr_Sum04 { get; set; }

        public decimal? Hdr_Sum05 { get; set; }

        public decimal? Hdr_Sum06 { get; set; }

        public decimal? Hdr_Sum07 { get; set; }

        public decimal? Hdr_Sum08 { get; set; }

        public decimal? Hdr_Sum09 { get; set; }

        public decimal? Hdr_Sum10 { get; set; }

        public decimal? Hdr_Sum11 { get; set; }

        public decimal? Hdr_Sum12 { get; set; }

        public short? Hdr_ExchType1 { get; set; }

        public short? Hdr_ExchType2 { get; set; }

        public short? Hdr_ExchType3 { get; set; }

        public short? Hdr_ExchType4 { get; set; }

        public decimal? Hdr_ExchRate1 { get; set; }

        public decimal? Hdr_ExchRate2 { get; set; }

        public decimal? Hdr_ExchRate3 { get; set; }

        public decimal? Hdr_ExchRate4 { get; set; }

        public decimal? Hdr_ExchSum01 { get; set; }

        public decimal? Hdr_ExchSum02 { get; set; }

        public decimal? Hdr_ExchSum03 { get; set; }

        public decimal? Hdr_ExchSum04 { get; set; }

        public decimal? Hdr_ExchSum05 { get; set; }

        public decimal? Hdr_ExchSum06 { get; set; }

        public decimal? Hdr_ExchSum07 { get; set; }

        public decimal? Hdr_ExchSum08 { get; set; }

        public decimal? Hdr_ExchSum09 { get; set; }

        public decimal? Hdr_ExchSum10 { get; set; }

        public decimal? Hdr_ExchSum11 { get; set; }

        public decimal? Hdr_ExchSum12 { get; set; }

        public int? Hdr_Value0_01 { get; set; }

        public int? Hdr_Value0_02 { get; set; }

        public int? Hdr_Value0_03 { get; set; }

        public int? Hdr_Value0_04 { get; set; }

        public int? Hdr_Value0_05 { get; set; }

        public int? Hdr_Value0_06 { get; set; }

        public int? Hdr_Value0_07 { get; set; }

        public int? Hdr_Value0_08 { get; set; }

        public int? Hdr_Value0_09 { get; set; }

        public int? Hdr_Value0_10 { get; set; }

        public decimal? Hdr_Value2_01 { get; set; }

        public decimal? Hdr_Value2_02 { get; set; }

        public decimal? Hdr_Value2_03 { get; set; }

        public decimal? Hdr_Value2_04 { get; set; }

        public decimal? Hdr_Value2_05 { get; set; }

        public decimal? Hdr_Value2_06 { get; set; }

        public decimal? Hdr_Value2_07 { get; set; }

        public decimal? Hdr_Value2_08 { get; set; }

        public decimal? Hdr_Value2_09 { get; set; }

        public decimal? Hdr_Value2_10 { get; set; }

        public decimal? Hdr_Value4_01 { get; set; }

        public decimal? Hdr_Value4_02 { get; set; }

        public decimal? Hdr_Value4_03 { get; set; }

        public decimal? Hdr_Value4_04 { get; set; }

        public decimal? Hdr_Value4_05 { get; set; }

        public decimal? Hdr_Value4_06 { get; set; }

        public decimal? Hdr_Value4_07 { get; set; }

        public decimal? Hdr_Value4_08 { get; set; }

        public decimal? Hdr_Value4_09 { get; set; }

        public decimal? Hdr_Value4_10 { get; set; }

        public string Hdr_Misc1 { get; set; }

        public string Hdr_Misc2 { get; set; }

        public string Hdr_Misc3 { get; set; }

        public string Hdr_Misc4 { get; set; }

        public string Hdr_Misc5 { get; set; }

        public string Hdr_Misc6 { get; set; }

        public DateTime? Hdr_Date1 { get; set; }

        public DateTime? Hdr_Date2 { get; set; }

        public DateTime? Hdr_Date3 { get; set; }

        public DateTime? Hdr_Date4 { get; set; }

        public DateTime? Hdr_Date5 { get; set; }

        public DateTime? Hdr_Date6 { get; set; }

        public bool Hdr_BoolFld01YN { get; set; }

        public bool Hdr_BoolFld02YN { get; set; }

        public bool Hdr_BoolFld03YN { get; set; }

        public string Hdr_Cmnt { get; set; }

        public bool Hdr_UsrChFlag { get; set; }
        [Key]
        public Guid Hdr_UniCode { get; set; }

        public Guid? Hdr_ParentUniCode { get; set; }

        //public int Hdr_IDN { get; set; }

        public DateTime? ModifyDateTime { get; set; }

        public DateTime? LoadDateTime { get; set; }

        public DateTime? UnloadDateTime { get; set; }

        public byte[] Version { get; set; }
    }
}