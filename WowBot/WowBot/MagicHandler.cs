using Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class MagicHandler
	{
		public static BlackMagic BMWow;

		static MagicHandler()
		{
			BMWow = new BlackMagic();
		}
	}
}
