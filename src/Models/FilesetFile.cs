// --------------------------------------------------------------------------------
// <copyright file="FilesetFile.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the FilesetFile type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    using System.Collections.Generic;

    public class FilesetFile
    {
        private bool isActive = true;
        #region Constructors and Destructors

        public FilesetFile()
        {
            this.Variables = new Dictionary<string, string>();
            this.IsActiveExpr = "true";
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public Dictionary<string, string> Variables { get; private set; }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (value.Equals(this.isActive))
                {
                    return;
                }

                this.isActive = value;
                //this.IsVisible = value;
                //this.OnPropertyChanged();
            }
        }

        public string IsActiveExpr { get; set; }

        #endregion
    }
}
