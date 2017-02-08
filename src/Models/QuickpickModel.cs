namespace SpitOut.Models
{
    using System.Collections.Generic;

    public class QuickpickModel
    {
        #region Constructors and Destructors

        public QuickpickModel()
        {
            this.Presets = new Dictionary<string, string>();
        }

        #endregion

        #region Public Properties

        public string Label { get; set; }

        public Dictionary<string, string> Presets { get; private set; }

        #endregion
    }
}