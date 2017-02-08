// --------------------------------------------------------------------------------
// <copyright file="Fileset.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the Fileset type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    using System.Collections.Generic;

    public class Fileset
    {
        #region Constructors and Destructors

        public Fileset()
        {
            this.Files = new List<FilesetFile>();
        }

        #endregion

        #region Public Properties

        public List<FilesetFile> Files { get; private set; }

        public string TemplateName { get; set; }

        #endregion
    }
}
