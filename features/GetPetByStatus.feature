@all @get_pet_by_status
Feature: GetPetByStatus

@280320201827 @pos
Scenario: Положительный тест: 3 статуса питомца
	* get pet by status "available"
	* get pet by status "pending"
	* get pet by status "sold"

@280320201828 @neg
Scenario: Негативный тест: 3 статуса питомца
	* get pet by status "i"
	* get pet by status ""
	* get pet by status "SolD"