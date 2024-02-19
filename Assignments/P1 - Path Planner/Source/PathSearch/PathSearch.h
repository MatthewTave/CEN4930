#include "../platform.h" // This file will make exporting DLL symbols simpler for students.

#include "../Framework/TileSystem/Tile.h"
#include "../Framework/TileSystem/TileMap.h"
#include "../PriorityQueue.h"

#include <vector>
using namespace std;

namespace ufl_cap4053
{
	namespace searches
	{
		class PathSearch
		{
		private:
			TileMap* map;

			pair<int, int> curr , s, t;
			
			vector<vector<int>> cost;
			vector<vector<pair<int, int>>> prev;

			PriorityQueue<pair<int, int>> q;

		public:
			DLLEXPORT PathSearch();
			DLLEXPORT ~PathSearch();

			DLLEXPORT void load(TileMap* _tileMap);
			DLLEXPORT void initialize(int startRow, int startCol, int goalRow, int goalCol);
			DLLEXPORT void update(long timeslice);
			DLLEXPORT void shutdown();
			DLLEXPORT void unload();

			DLLEXPORT bool isDone() const;
			DLLEXPORT std::vector<Tile const*> const getSolution() const;
		};
	}
}  // close namespace ufl_cap4053::searches
