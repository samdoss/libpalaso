﻿using System;
using System.Collections.Generic;

namespace SIL.WritingSystems
{
	/// <summary>
	/// This interface defines the functions of a keyboard controller that are required to implement the keyboard-related
	/// methods of WritingSystem.
	/// Various classes obtain one of these when needed by reading Keyboard.Controller.
	/// The default implementation of this in the core Palaso DLL has minimal functionality.
	/// Typically clients will set Keyboard.Controller to some more useful class, such as UI.WindowsForms.KeyboardController.
	/// </summary>
	public interface IKeyboardController: IDisposable
	{
		/// <summary>
		/// Tries to get the keyboard with the specified <paramref name="layoutNameWithLocale"/>.
		/// </summary>
		IKeyboardDefinition GetKeyboard(string layoutNameWithLocale);

		/// <summary>
		/// Tries to get the keyboard with the specified <paramref name="layoutName"/>
		/// and <paramref name="locale"/>.
		/// </summary>
		IKeyboardDefinition GetKeyboard(string layoutName, string locale);

		/// <summary>
		/// Tries to get the keyboard for the specified <paramref name="writingSystem"/>.
		/// </summary>
		IKeyboardDefinition GetKeyboard(WritingSystemDefinition writingSystem);

		/// <summary>
		/// Tries to get the keyboard for the specified <paramref name="language"/>.
		/// </summary>
		IKeyboardDefinition GetKeyboard(IInputLanguage language);

		/// <summary>
		/// Activates the keyboard
		/// </summary>
		void SetKeyboard(IKeyboardDefinition keyboard);

		/// <summary>
		/// Activates the keyboard
		/// </summary>
		void SetKeyboard(string layoutName);

		/// <summary>
		/// Activates the keyboard
		/// </summary>
		void SetKeyboard(string layoutName, string locale);

		/// <summary>
		/// Activates the keyboard
		/// </summary>
		void SetKeyboard(WritingSystemDefinition writingSystem);

		/// <summary>
		/// Activates the keyboard
		/// </summary>
		void SetKeyboard(IInputLanguage language);

		/// <summary>
		/// Activates the keyboard of the default input language
		/// </summary>
		void ActivateDefaultKeyboard();

		/// <summary>
		/// Returns everything that is installed on the system and available to be used.
		/// This would typically be used to populate a list of available keyboards in configuring a writing system.
		/// Currently it is also used to evaluate whether a particular keyboard is available to use on this system.
		/// Enhance: the latter task could be done more efficiently with another API function to indicate whether
		/// a particular keyboard is available. Not sure whether the cost of building and searching the list is enough
		/// to make this worthwhile.
		/// </summary>
		IEnumerable<IKeyboardDefinition> AllAvailableKeyboards { get; }

		/// <summary>
		/// Call when something may have changed in the external world that should affect AllAvailableKeyboards
		/// (e.g., when activating a window that needs an accurate current list).
		/// </summary>
		void UpdateAvailableKeyboards();

		/// <summary>
		/// Figures out the system default keyboard for the specified writing system (the one to use if we have no available KnownKeyboards).
		/// The implementation may use obsolete fields such as Keyboard
		/// </summary>
		IKeyboardDefinition DefaultForWritingSystem(WritingSystemDefinition ws);

		/// <summary>
		/// Finds a keyboard specified using one of the legacy fields. If such a keyboard is found, it is appropriate to
		/// automatically add it to KnownKeyboards. If one is not, a general DefaultKeyboard should NOT be added.
		/// This is intended to be used when KnownKeyboards is empty. It may return null.
		/// </summary>
		IKeyboardDefinition LegacyForWritingSystem(WritingSystemDefinition ws);
		/// <summary>
		/// Creates and returns a keyboard definition object based on the layout and locale.
		/// </summary>
		IKeyboardDefinition CreateKeyboardDefinition(string layout, string locale);

		/// <summary>
		/// Gets or sets the currently active keyboard
		/// </summary>
		IKeyboardDefinition ActiveKeyboard { get; set; }
	}
}
