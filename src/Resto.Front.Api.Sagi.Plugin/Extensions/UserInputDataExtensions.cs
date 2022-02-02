using Resto.Front.Api.Exceptions;
using Resto.Front.Api.Sagi.Plugin.Constants;

namespace Resto.Front.Api.Sagi.Plugin.Extensions
{
	public static class UserInputDataExtensions
	{
		public static string ToCodeString(this string input)
		{
			if (input.Length != Consts.CodeLength)
				throw new PaymentActionFailedException(Resources.TitleCodeInput);

			return input.Insert(Consts.CodeLength / 2, "%20");
		}
	}
}