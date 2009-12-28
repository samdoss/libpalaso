using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Palaso.DictionaryServices;
using Palaso.Tests.Data;
using Palaso.TestUtilities;
using Palaso.DictionaryServices.Model;

namespace WeSay.LexicalModel.Tests
{

	internal static class LiftFileInitializer
	{
		public static string MakeFile(string liftFileName)
		{
			File.WriteAllText(liftFileName,
							  @"<?xml version='1.0' encoding='utf-8'?>
				<lift
					version='0.13'
					producer='WeSay 1.0.0.0'>
					<entry
						id='Sonne_c753f6cc-e07c-4bb1-9e3c-013d09629111'
						dateCreated='2008-07-01T06:29:23Z'
						dateModified='2008-07-01T06:29:57Z'
						guid='c753f6cc-e07c-4bb1-9e3c-013d09629111'>
						<lexical-unit>
							<form
								lang='v'>
								<text>Sonne</text>
							</form>
						</lexical-unit>
						<sense
							id='33d60091-ba96-4204-85fe-9d15a24bd5ff'>
							<trait
								name='SemanticDomainDdp4'
								value='1 Universe, creation' />
						</sense>
					</entry>
				</lift>");
			return liftFileName;
		}
	}

	[TestFixture]
	public class LiftLexEntryRepositoryCreatedFromPersistedData :
		IRepositoryPopulateFromPersistedTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LiftLexEntryRepositoryCreatedFromPersistedData");
			_persistedFilePath = LiftFileInitializer.MakeFile(_tempFolder.GetTemporaryFile());
			DataMapperUnderTest = new LiftLexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public override void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreSave = DataMapperUnderTest.LastModified;
			MakeItemDirty(Item);
			DataMapperUnderTest.SaveItem(Item);
			Assert.Greater(DataMapperUnderTest.LastModified, modifiedTimePreSave);
		}

		[Test]
		public override void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<LexEntry> itemsToSave = new List<LexEntry>();
			itemsToSave.Add(Item);
			DateTime modifiedTimePreSave = DataMapperUnderTest.LastModified;
			MakeItemDirty(Item);
			DataMapperUnderTest.SaveItems(itemsToSave);
			Assert.Greater(DataMapperUnderTest.LastModified, modifiedTimePreSave);
		}

		private static void MakeItemDirty(LexEntry Item)
		{
			Item.LexicalForm["de"] = "Sonne";
		}

		protected override void  LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
		{
			SetState();
			Assert.AreEqual(Item.ModificationTime, DataMapperUnderTest.LastModified);
		}

		[Test]
		public void Constructor_LexEntryIsDirtyIsFalse()
		{
			SetState();
			Assert.IsFalse(Item.IsDirty);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LiftLexEntryRepository(_persistedFilePath);
		}
	}
}