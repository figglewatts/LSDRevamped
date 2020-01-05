namespace LSDR.Entities.Dream
{
	/// <summary>
	/// Stores texture set indices for texture sets (Normal, Kanji, etc...).
	/// </summary>
	public enum TextureSet
	{
		Normal,
		Kanji,
		Downer,
		Upper
	}
	
	/*
	 * TODO: Improve how texture sets work -- I think the following would be good:
	 * During the course of the game the Tex variable increases its value up to 3 and then resets back to 0.
     * What happens is outlined below:
     * Tex = 0, only TexSet A appear.
	 * Tex=1, TexSet B can appear alongside TexSet A.
	 * Tex=2, TexSet C now has a chance to appear too.
	 * Tex=3, All TexSets can now appear.
	 * It may feel like when tex=1, TexSet B has more chance to appear than TexSet A, but I believe that the 
	 * chances are pretty much random, or if there is a pattern, is not as simple as "Tex's value determines
	 * the TexSet with the highest chance to appear"
	 * Source: https://dreamemulator.fandom.com/wiki/Textures#comm-8106
	 */
}
