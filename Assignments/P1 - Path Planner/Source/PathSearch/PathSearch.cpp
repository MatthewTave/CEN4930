#include "PathSearch.h"
using namespace std;

namespace ufl_cap4053
{
	namespace searches
	{
		PathSearch::PathSearch() {
			return;
		}

		PathSearch::~PathSearch() {
			return;
		}

		void PathSearch::load(TileMap* _tileMap) {
			return;
		}

		void PathSearch::initialize(int startRow, int startCol, int goalRow, int goalCol) {
			return;
		}

		void PathSearch::update(long timeslice) {
			return;
		}

		void PathSearch::shutdown() {
			return;
		}

		void PathSearch::unload() {
			return;
		}

		bool PathSearch::isDone() const {
			return false;
		}

		vector<Tile const*> const PathSearch::getSolution() const {
			return vector<Tile const*>();
		}
	}
}  // close namespace ufl_cap4053::searches
