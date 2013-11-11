#if UNITY_METRO && NETFX_CORE && !UNITY_EDITOR
using System.Text.RegularExpressions;

namespace System.Net.Mail {
	public class MailAddress {		
		/**
		 * Not the best address validator but probably better than MailAddress'.
		 * From http://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
		 * See also: http://stackoverflow.com/questions/7173401/c-sharp-email-validation-confused-by-mailaddress-behavior-johngmail-is-val
		 */
    	private static Regex validEmailRegex = new Regex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);
    
		public MailAddress(string address) {
			if(address == null)
				throw new ArgumentNullException("Email address is null");
			else if(address.Equals(String.Empty))
				throw new ArgumentException("Email address is empty");
			else if(!validEmailRegex.IsMatch(address))
				throw new FormatException("Email address is not in a recognized format");
		}
	}
}
#endif