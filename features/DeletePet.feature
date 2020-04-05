@all @delete_pet
Feature: DeletePet

@020420200515 @DShapochkin
Scenario: Delete a pet and ensure it is gone
	* create pet with name "Cat" and photourls "test.com"
	* delete pet
	* ensure code is 200
	* ensure pet is gone
	* delete pet
	* ensure code is 404
