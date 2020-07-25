@mvc
Feature: DELETE pass removal

	@idDelete1
	Scenario: Delete existing pass
		* add pass with random person
		* delete the pass by guid
		* assert code is 200

	@idDelete2
	Scenario: Delete non-existing pass
		* add pass with random person
		* delete the pass by guid
		* delete the pass by guid
		* assert code is 404