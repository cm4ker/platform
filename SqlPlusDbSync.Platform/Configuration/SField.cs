namespace SqlPlusDbSync.Platform.Configuration
{
    public class SField : IArgument
    {
        private string _fullName = null;

        public string Name { get; set; }


        /// <summary>
        /// Alias for property. This take effect on all modules.
        /// </summary>
        public string Alias { get; set; }

        public bool IsIdentifier { get; set; }


        public SType Owner { get; set; }
        public SPoint Point { get; set; }
        public SPoint OwnerPoint { get; set; }
        public SSchema Schema { get; set; }
        
        public string GetFullName()
        {
            if (_fullName is null)
                _fullName = Owner.GetFullName() + "." + Name;

            return _fullName;
        }

        public SField GetCopyFieldWithOwner(SType owner)
        {
            var clone = this.MemberwiseClone() as SField;
            clone.Owner = owner;
            return clone;
        }
    }
}