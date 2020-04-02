@all @get_pet_by_status
Feature: GetPetByStatus

@280320201827 @pos @DShapochkin
Scenario: Positive test: all 3 correct statuses
	* get pet by status "available"
	* get pet by status "pending"
	* get pet by status "sold"

@280320201828 @neg @DShapochkin
Scenario: Negative test: 3 wrong statuses
	* get pet by status "i"
	* get pet by status ""
	* get pet by status "SolD"