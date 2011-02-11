﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp
{
	public abstract class AttributedNode : AstNode
	{
		public static readonly Role<AttributeSection> AttributeRole = new Role<AttributeSection>("Attribute");
		public static readonly Role<CSharpModifierToken> ModifierRole = new Role<CSharpModifierToken>("Modifier");
		
		public IEnumerable<AttributeSection> Attributes {
			get { return base.GetChildrenByRole (AttributeRole); }
			set { SetChildrenByRole (AttributeRole, value); }
		}
		
		public Modifiers Modifiers {
			get { return GetModifiers(this); }
			set { SetModifiers(this, value); }
		}
		
		internal static Modifiers GetModifiers(AstNode node)
		{
			Modifiers m = 0;
			foreach (CSharpModifierToken t in node.GetChildrenByRole (ModifierRole)) {
				m |= t.Modifier;
			}
			return m;
		}
		
		internal static void SetModifiers(AstNode node, Modifiers newValue)
		{
			Modifiers oldValue = GetModifiers(node);
			AstNode insertionPos = node.GetChildrenByRole(AttributeRole).LastOrDefault();
			foreach (Modifiers m in CSharpModifierToken.AllModifiers) {
				if ((m & newValue) != 0) {
					if ((m & oldValue) == 0) {
						// Modifier was added
						node.InsertChildAfter(insertionPos, new CSharpModifierToken(AstLocation.Empty, m), ModifierRole);
					} else {
						// Modifier already exists
						insertionPos = node.GetChildrenByRole(ModifierRole).First(t => t.Modifier == m);
					}
				} else {
					if ((m & oldValue) != 0) {
						// Modifier was removed
						node.GetChildrenByRole (ModifierRole).First(t => t.Modifier == m).Remove();
					}
				}
			}
		}
	}
}