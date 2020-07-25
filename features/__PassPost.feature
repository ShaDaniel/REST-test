@mvc
Feature: POST pass creation

	@idPost1
	Scenario: Create pass
		* add pass with random person
		* assert code is 200
		* get by guid and assert return code is 200
