@mvc
Feature: GET pass information

	@idGet1
	Scenario: try to get existing pass info
		* add pass with random person
		* get by guid and assert return code is 200

	@idGet2
	Scenario: try to get non-existing pass info
		* generate random guid
		* delete the pass by guid
		* get by guid and assert return code is 404
