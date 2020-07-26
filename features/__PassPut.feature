@mvc
Feature: PUT pass update

	@idPut1
	Scenario: update existing pass
		* add pass with random person
		* update pass with random info
		* assert code is 200
		* get by guid and assert return code is 200

	@idPut2
	Scenario: update non-existing pass
		* generate random guid
		* delete the pass by guid
		* update pass with random info
		* assert code is 404
