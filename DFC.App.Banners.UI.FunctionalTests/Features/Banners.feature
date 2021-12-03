Feature: Banners

@Banners @Smoke
Scenario: Navigate to Job Group page
	Given I am on the nurse job profile page
	When I click the Explore job trends for this group link
	Then I am on the Job group: Nurses page

	@Banners
	Scenario: Job Growth information is displayed
	Given I am on the nurse job profile page
	When I click the Explore job trends for this group link
	Then I am on the Job group: Nurses page
	And the Job growth information is displayed

	@Banners
	Scenario: Qualifications information is displayed
	Given I am on the nurse job profile page
	When I click the Explore job trends for this group link
	Then I am on the Job group: Nurses page
	And the Qualifications information is displayed

	@Banners
	Scenario: Regional information is displayed
	Given I am on the nurse job profile page
	When I click the Explore job trends for this group link
	Then I am on the Job group: Nurses page
	And the Regional information is displayed

	@Banners
	Scenario: Industry information is displayed
	Given I am on the nurse job profile page
	When I click the Explore job trends for this group link
	Then I am on the Job group: Nurses page
	And the Industry information is displayed