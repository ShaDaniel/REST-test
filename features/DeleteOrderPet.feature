@all @delete_order
Feature: DeleteOrderPet

@020420200625 @DShapochkin
Scenario: Delete an order for a pet
	* create pet with name "Cat" and photourls "test.com"
	* delete pet order
	* ensure code is 200
	* ensure that deletion order is ok
	* delete pet order
	* ensure code is 404
