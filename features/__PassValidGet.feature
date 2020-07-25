@mvc
Feature: GET pass validation
			New Pass is valid for 2 days under the current settings

	@idGetValid1
	Scenario: create pass and check validity
		* add pass with random person
		* get the pass status
		* assert code is 200

	@idGetValid1
	Scenario: delete pass and check validity
		* add pass with random person
		* delete the pass by guid
		* get the pass status
		* assert code is 404

		
