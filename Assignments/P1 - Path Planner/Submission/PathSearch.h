#include "../platform.h" // This file will make exporting DLL symbols simpler for students.

#include "../Framework/TileSystem/Tile.h"
#include "../Framework/TileSystem/TileMap.h"
#include "../PriorityQueue.h"

#include <vector>
using namespace std;

// Struct to store nodes for the queue
struct pathNode {
	pair<int, int> coords, t;
	int terrainCost, totalCost;

	pathNode(pair<int, int> coords, int terrainCost, int totalCost, pair<int, int> t) : coords(coords), terrainCost(terrainCost), totalCost(totalCost), t(t) {}
};

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

			PriorityQueue<pathNode> q;

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
