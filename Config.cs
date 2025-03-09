namespace SLCosmetics.Config
{
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;
    using UnityEngine;

    public class Config : IConfig
    {
        /// <summary>
        ///  Will the plugin run?
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        ///  Will the plugin print Debug Text?
        /// </summary>
        public bool Debug { get; set; } = false;
    }
}
