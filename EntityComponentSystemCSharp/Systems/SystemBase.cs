using System;
using System.Collections.Generic;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public abstract class SystemBase: ISystem
	{
		protected readonly EntityManager _em;
		public SystemBase(EntityManager em)
		{
			_em = em;
		}
		public abstract void Run();

	}
}