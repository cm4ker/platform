using System;

namespace SqlPlusDbSync.Data.Model
{
    public class StrInfo
    {
        public int Cmp_UniCode { get; set; }
        public string Str_BarCode { get; set; }
        public int? ObjCode { get; set; }
        public short Str_DecPlace { get; set; }
        public decimal Str_Price01 { get; set; }
        public decimal Str_Price02 { get; set; }
        public decimal Str_Price03 { get; set; }
        public decimal Str_Price04 { get; set; }
        public decimal Str_Price05 { get; set; }
        public decimal Str_Price06 { get; set; }
        public decimal Str_Price07 { get; set; }
        public decimal Str_Price08 { get; set; }
        public decimal Str_Price09 { get; set; }
        public decimal Str_Price10 { get; set; }
        public decimal Str_Price11 { get; set; }
        public decimal Str_Price12 { get; set; }
        public decimal Str_ExchPrice01 { get; set; }
        public decimal Str_ExchPrice02 { get; set; }
        public decimal Str_ExchPrice03 { get; set; }
        public decimal Str_ExchPrice04 { get; set; }
        public decimal Str_ExchPrice05 { get; set; }
        public decimal Str_ExchPrice06 { get; set; }
        public decimal Str_ExchPrice07 { get; set; }
        public decimal Str_ExchPrice08 { get; set; }
        public decimal Str_ExchPrice09 { get; set; }
        public decimal Str_ExchPrice10 { get; set; }
        public decimal Str_ExchPrice11 { get; set; }
        public decimal Str_ExchPrice12 { get; set; }
        public decimal Str_CurrPrice01 { get; set; }
        public decimal Str_CurrPrice02 { get; set; }
        public decimal Str_CurrPrice03 { get; set; }
        public decimal Str_CurrPrice04 { get; set; }
        public decimal Str_CurrPrice05 { get; set; }
        public decimal Str_CurrPrice06 { get; set; }
        public decimal Str_CurrPrice07 { get; set; }
        public decimal Str_CurrPrice08 { get; set; }
        public decimal Str_CurrPrice09 { get; set; }
        public decimal Str_CurrPrice10 { get; set; }
        public decimal Str_CurrPrice11 { get; set; }
        public decimal Str_CurrPrice12 { get; set; }
        /// <summary>
        /// Признак НДС
        /// </summary>
        public int Str_Value0_01 { get; set; }
        /// <summary>
        /// Код производителя
        /// </summary>
        public int Str_Value0_02 { get; set; }
        /// <summary>
        /// Код страны
        /// </summary>
        public int Str_Value0_03 { get; set; }
        /// <summary>
        /// Признак НСП
        /// </summary>
        public int Str_Value0_04 { get; set; }
        public int Str_Value0_05 { get; set; }
        public int Str_Value0_06 { get; set; }
        public int Str_Value0_07 { get; set; }
        public int Str_Value0_08 { get; set; }
        public int Str_Value0_09 { get; set; }
        public int Str_Value0_11 { get; set; }
        public string Str_Value0_12 { get; set; }
        /// <summary>
        /// % розн. наценки
        /// </summary>
        public decimal? Str_Value2_01 { get; set; }
        /// <summary>
        /// % опт.  наценки
        /// </summary>
        public decimal? Str_Value2_02 { get; set; }
        /// <summary>
        /// % торг. надбавки импортёра
        /// </summary>
        public decimal? Str_Value2_03 { get; set; }
        /// <summary>
        /// % НДС
        /// </summary>
        public decimal? Str_Value2_04 { get; set; }
        public decimal? Str_Value2_05 { get; set; }
        public decimal? Str_Value2_06 { get; set; }
        public decimal? Str_Value2_07 { get; set; }
        public decimal? Str_Value2_08 { get; set; }
        public decimal? Str_Value2_09 { get; set; }
        public decimal? Str_Value2_10 { get; set; }
        public decimal? Str_Value2_11 { get; set; }
        public decimal? Str_Value2_12 { get; set; }
        public decimal? Info_Value4_01 { get; set; }
        public decimal? Info_Value4_02 { get; set; }
        public decimal? Info_Value4_03 { get; set; }
        public decimal? Info_Value4_04 { get; set; }
        public decimal? Info_Value4_05 { get; set; }
        public decimal? Info_Value4_06 { get; set; }
        public decimal? Info_Value4_07 { get; set; }
        public decimal? Info_Value4_08 { get; set; }
        public decimal? Info_Value4_09 { get; set; }
        public decimal? Info_Value4_10 { get; set; }
        public decimal? Info_Value4_11 { get; set; }
        public int? Info_Value4_12 { get; set; }
        /// <summary>
        /// № ГТД
        /// </summary>
        public string Info_Str_01 { get; set; }
        /// <summary>
        /// Штрих-код производителя
        /// </summary>
        public string Info_Str_02 { get; set; }
        /// <summary>
        /// Кем выдан сертификат
        /// </summary>
        public string Info_Str_03 { get; set; }
        public string Info_Str_04 { get; set; }
        public string Info_Str_05 { get; set; }
        public string Info_Str_06 { get; set; }
        public string Info_Str_07 { get; set; }
        public string Info_Str_08 { get; set; }
        public string Info_Str_09 { get; set; }
        public string Info_Str_10 { get; set; }
        public string Info_Str_11 { get; set; }
        public string Info_Str_12 { get; set; }
        public int? Str_QntInBox { get; set; }
        public decimal? Str_Weight { get; set; }
        public bool Str_SertifNeedYN { get; set; }
        public string Str_SertifNum { get; set; }
        public DateTime? Str_SertifDate1 { get; set; }
        public DateTime? Str_SertifDate2 { get; set; }
        public string Str_SeriesNum { get; set; }
        public DateTime? Str_DatePrepare { get; set; }
        public DateTime? Str_DateSuitable { get; set; }
        public Guid InfoID { get; set; }
        public DateTime? Info_Date1 { get; set; }
        public DateTime? Info_Date2 { get; set; }
        public DateTime? Info_Date3 { get; set; }
        public Guid? Str_Value0_10 { get; set; }
        //public int Info_IDN { get; set; }
        public DateTime? ModifyDateTime { get; set; }
        public DateTime? LoadDateTime { get; set; }
        public DateTime? UnloadDateTime { get; set; }
    }
}