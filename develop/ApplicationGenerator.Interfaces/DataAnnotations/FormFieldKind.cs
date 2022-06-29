// file:	DataAnnotations\FormFieldKind.cs
//
// summary:	Implements the form field kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent form field kinds. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public enum FormFieldKind
    {
        /// <summary>   An enum constant representing the data type option. </summary>
        DataType,
        /// <summary>   An enum constant representing the dropdown option. </summary>
        Dropdown,
        /// <summary>   An enum constant representing the captcha option. </summary>
        Captcha,
        /// <summary>   An enum constant representing the radio option. </summary>
        Radio,
        /// <summary>   An enum constant representing the label option. </summary>
        Label,
        /// <summary>   An enum constant representing the hidden option. </summary>
        Hidden,
        /// <summary>   An enum constant representing the range slider option. </summary>
        RangeSlider,
        /// <summary>   An enum constant representing the spinner option. </summary>
        Spinner,
        /// <summary>   An enum constant representing the user name option. </summary>
        UserName,
        /// <summary>   An enum constant representing the password option. </summary>
        Password,
        /// <summary>   An enum constant representing the password repeat option. </summary>
        PasswordRepeat
    }
}
