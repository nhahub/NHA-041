// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ============================================
// LOGIN REQUIRED MODAL - Anonymous User Actions
// ============================================
function showLoginModal(returnUrl) {
    var loginModal = document.getElementById('loginRequiredModal');
    var loginUrl = '/Account/login';
    if (returnUrl && returnUrl !== '#' && returnUrl !== '') {
        loginUrl += '?ReturnUrl=' + encodeURIComponent(returnUrl);
    }

    var loginBtn = document.getElementById('loginRedirectBtn');
    if (loginBtn) {
        loginBtn.href = loginUrl;
    }

    if (loginModal && typeof bootstrap !== 'undefined') {
        try {
            var modal = new bootstrap.Modal(loginModal);
            modal.show();
            return true;
        } catch (e) {
            console.error('Error showing modal:', e);
        }
    }
    
    // Fallback: redirect to login
    window.location.href = loginUrl;
    return false;
}

(function () {

    // Handle clicks on .require-login elements
    document.addEventListener('click', function (e) {
        var target = e.target;
        
        // Walk up the DOM tree to find .require-login
        while (target && target !== document) {
            if (target.classList && target.classList.contains('require-login')) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                
                var returnUrl = target.getAttribute('data-return-url') || target.getAttribute('href') || window.location.href;
                showLoginModal(returnUrl);
                return false;
            }
            target = target.parentElement;
        }
    }, true); // Use capture phase

    // Handle form submissions on .require-login-form
    document.addEventListener('submit', function (e) {
        var target = e.target;
        
        while (target && target !== document) {
            if (target.classList && target.classList.contains('require-login-form')) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                
                var returnUrl = target.getAttribute('data-return-url') || window.location.href;
                showLoginModal(returnUrl);
                return false;
            }
            target = target.parentElement;
        }
    }, true); // Use capture phase
})();
function deleteconfirm(UserId, IsdDeletecClicked) {
    var spanid = "deleteConfirmSpan" + UserId;
    var deletespan = "deletespan" + UserId;
    var spanEl = document.getElementById(spanid);
    var deleteEl = document.getElementById(deletespan);

    if (!spanEl || !deleteEl) {
        console.error("Element not found for UserId: " + UserId + ". Looking for: " + spanid + ", " + deletespan);
        return;
    }

    if (IsdDeletecClicked) {
        spanEl.style.display = "block";
        deleteEl.style.display = "none";
    }
    else {
        spanEl.style.display = "none";
        deleteEl.style.display = "block";
    }
}

function submitDeletePayment(paymentMethodId) {
    var form = document.getElementById('delete-payment-form');
    var input = document.getElementById('delete-payment-id');
    if (form && input) {
        input.value = paymentMethodId;
        form.submit();
    } else {
        console.error("Delete form or input not found");
    }
}

// Function to toggle checkout button visibility based on cart count
function updateCheckoutButton() {
    const cartCountEl = document.getElementById("cart-count");
    const checkoutBtn = document.querySelector("#cart-modal-footer a");
    if (cartCountEl && checkoutBtn) {
        const count = parseInt(cartCountEl.getAttribute("data-count") || cartCountEl.innerText) || 0;
        if (count > 0) {
            checkoutBtn.style.display = ""; // Restore default display
        } else {
            checkoutBtn.style.display = "none"; // Hide button
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // Use event delegation to handle all forms (works for dynamically added forms too)
    document.addEventListener("submit", function (event) {
        const form = event.target;

        // --- 1. Handle Add Product Form ---
        if (form.tagName === "FORM" && (form.id === "add-product" || form.action.includes("Order/Create") || form.action.includes("Order/create"))) {
            event.preventDefault();
            const formData = new FormData(form);

            fetch("/Order/Create", {
                method: "POST",
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.message === "success" || data.Message === "success") {
                        const cartCount = data.cartCount || data.CartCount;
                        const cartCountEl = document.getElementById("cart-count");
                        if (cartCountEl && cartCount !== undefined) {
                            cartCountEl.innerText = cartCount;
                            cartCountEl.setAttribute("data-count", cartCount);
                        }

                        updateCheckoutButton(); // Update button visibility
                        showToast("Product added to cart successfully!", "success");
                    } else {
                        showToast("Failed to add product to cart.", "error");
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    showToast("An error occurred while adding the product.", "error");
                });
        }

        // --- 2. Handle Remove Product Form ---
        if (form.tagName === "FORM" && (form.id === "remove-product" || form.action.includes("Order/remove"))) {
            event.preventDefault();
            const formData = new FormData(form);

            fetch(form.action, {
                method: "POST",
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.message === "success" || data.Message === "success") {
                        // Update cart count
                        const cartCount = data.cartCount || data.CartCount;
                        const cartCountEl = document.getElementById("cart-count");
                        if (cartCountEl && cartCount !== undefined) {
                            cartCountEl.innerText = cartCount;
                            cartCountEl.setAttribute("data-count", cartCount);
                        }

                        updateCheckoutButton(); // Update button visibility
                        showToast("Product removed from cart.", "success");

                        // If we are on the checkout page, reload to update the summary
                        if (window.location.pathname.toLowerCase().includes("/transaction/proccesspayment")) {
                            window.location.reload();
                            return; // Stop further execution to let reload happen
                        }

                        // Refresh the cart modal to show updated list
                        const modalBody = document.getElementById("cart-modal-body");
                        if (modalBody) {
                            fetch("/Order/orderdetails", {
                                method: "GET",
                                headers: { "X-Requested-With": "XMLHttpRequest" }
                            })
                                .then(response => response.text())
                                .then(html => {
                                    modalBody.innerHTML = html;
                                })
                                .catch(err => console.error("Error refreshing cart:", err));
                        }

                    } else {
                        showToast(data.message || "Failed to remove product.", "error");
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    showToast("An error occurred while removing the product.", "error");
                });
        }
    });
});

// Cart Modal Functionality
document.addEventListener("DOMContentLoaded", function () {

    // Get all the modal elements
    const openCartButton = document.getElementById("open-cart-button");
    const closeCartButton = document.getElementById("cart-close-button");
    const cartOverlay = document.getElementById("cart-overlay");
    const cartModal = document.getElementById("cart-modal-container");
    const modalBody = document.getElementById("cart-modal-body");

    // --- Function to Open the Modal ---
    function openCartModal() {
        // Set a "loading" state
        if (modalBody) {
            modalBody.innerHTML = "<p>Loading cart contents...</p>";
        }

        // Show the overlay and modal
        if (cartOverlay) {
            cartOverlay.style.display = "block";
            // Trigger animation after display
            setTimeout(() => {
                cartOverlay.classList.add("show");
            }, 10);
        }

        if (cartModal) {
            cartModal.style.display = "flex";
            // Force reflow to ensure display change is applied
            cartModal.offsetHeight;
            // Trigger animation after display
            setTimeout(() => {
                cartModal.classList.add("show");
            }, 10);
        }

        // Prevent body scroll when modal is open
        document.body.style.overflow = "hidden";

        // Update checkout button visibility immediately
        updateCheckoutButton();

        // Fetch the cart HTML from our controller
        fetch("/Order/orderdetails", {
            method: "GET",
            headers: {
                // We don't need anti-forgery for a GET request
                "X-Requested-With": "XMLHttpRequest" // Identifies it as AJAX
            }
        }).then(response => response.text())
            .then(html => {
                modalBody.innerHTML = html;
            }).catch(error => {
                console.error("Error loading cart:", error);
                modalBody.innerHTML = "<p class='text-danger'>Sorry, we couldn't load your cart.</p>";
            });
    }

    // --- Function to Close the Modal ---
    function closeCartModal() {
        // Remove show class for animation
        if (cartOverlay) {
            cartOverlay.classList.remove("show");
        }
        if (cartModal) {
            cartModal.classList.remove("show");
        }

        // Wait for animation to complete before hiding
        setTimeout(() => {
            if (cartOverlay) {
                cartOverlay.style.display = "none";
            }
            if (cartModal) {
                cartModal.style.display = "none";
            }
            // Restore body scroll
            document.body.style.overflow = "";
        }, 300); // Match the CSS transition duration
    }

    // --- Attach Event Listeners ---

    // 1. Open the modal when the cart button is clicked
    if (openCartButton) {
        openCartButton.addEventListener("click", function (event) {
            event.preventDefault(); // Stop the link from navigating
            openCartModal();
        });
    }

    // 2. Close the modal when the 'X' button is clicked
    if (closeCartButton) {
        closeCartButton.addEventListener("click", closeCartModal);
    }

    // 3. Close the modal when the user clicks on the dark overlay
    if (cartOverlay) {
        cartOverlay.addEventListener("click", closeCartModal);
    }

    // 4. Prevent modal from closing when clicking inside it
    if (cartModal) {
        cartModal.addEventListener("click", function (event) {
            event.stopPropagation();
        });
    }
});

// Toast Notification Function
function showToast(message, type = "info") {
    // Remove existing toast if any
    const existingToast = document.querySelector('.toast-notification');
    if (existingToast) {
        existingToast.remove();
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast-notification ${type}`;
    toast.innerHTML = `
        <div class="toast-content">
            <p class="toast-message">${message}</p>
            <button class="toast-close" aria-label="Close">&times;</button>
        </div>
    `;

    // Add to page
    document.body.appendChild(toast);

    // Show toast
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);

    // Close button handler
    const closeBtn = toast.querySelector('.toast-close');
    closeBtn.addEventListener('click', () => {
        hideToast(toast);
    });

    // Click anywhere to close
    const clickHandler = (e) => {
        if (!toast.contains(e.target)) {
            hideToast(toast);
            document.removeEventListener('click', clickHandler);
        }
    };

    // Add click listener after a small delay to prevent immediate closing
    setTimeout(() => {
        document.addEventListener('click', clickHandler);
    }, 100);

    // Auto-hide after 5 seconds
    setTimeout(() => {
        hideToast(toast);
        document.removeEventListener('click', clickHandler);
    }, 5000);
}

function hideToast(toast) {
    toast.classList.add('hiding');
    setTimeout(() => {
        if (toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 300);
}

// Payment Form Handler
document.addEventListener("DOMContentLoaded", function () {
    const paymentForm = document.getElementById("payment-form");
    if (paymentForm) {
        paymentForm.addEventListener("submit", function (event) {
            event.preventDefault();

            const formData = new FormData(paymentForm);

            fetch("/Transaction/ProccessPayment", {
                method: "POST",
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.status === "succeeded" || data.status === "success") {
                        // Reset cart count to 0
                        const cartCountEl = document.getElementById("cart-count");
                        if (cartCountEl) {
                            cartCountEl.innerText = "0";
                            cartCountEl.setAttribute("data-count", "0");
                        }
                        showToast(data.message || "Payment processed successfully!", "success");
                        // Redirect to UserView after a short delay
                        setTimeout(() => {
                            window.location.href = "/Shelter/UserView";
                        }, 1500);
                    } else {
                        showToast(data.message || "Payment processing failed.", "error");
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    showToast("An error occurred while processing payment.", "error");
                });
        });
    }
});

// Payment Button Validation
document.addEventListener('DOMContentLoaded', function () {
    const paymentRadios = document.querySelectorAll('input[name="selectedPaymentMethodid"]');
    const submitBtn = document.getElementById('submit-payment-btn');

    function updateSubmitButton() {
        let isChecked = false;
        paymentRadios.forEach(radio => {
            if (radio.checked) {
                isChecked = true;
            }
        });

        if (submitBtn) {
            submitBtn.disabled = !isChecked;
        }
    }

    if (paymentRadios.length > 0) {
        paymentRadios.forEach(radio => {
            radio.addEventListener('change', updateSubmitButton);
        });

        // Run on load in case of browser auto-fill or back navigation
        updateSubmitButton();
    }
});

// Card Input Formatting and Detection
document.addEventListener('DOMContentLoaded', function () {
    const cardInput = document.getElementById('card-number-input');

    if (cardInput) {
        const cardIcon = document.getElementById('card-icon').querySelector('i');

        // Format card number with spaces
        cardInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, ''); // Remove non-digits
            let formattedValue = '';

            for (let i = 0; i < value.length; i++) {
                if (i > 0 && i % 4 === 0) {
                    formattedValue += ' ';
                }
                formattedValue += value[i];
            }

            e.target.value = formattedValue;

            // Detect Card Type
            if (value.startsWith('4')) {
                cardIcon.className = 'fab fa-cc-visa text-primary fa-lg'; // Visa Blue
                cardIcon.style.color = '#1A1F71';
            } else if (value.startsWith('5')) {
                cardIcon.className = 'fab fa-cc-mastercard text-warning fa-lg'; // Mastercard Orange
                cardIcon.style.color = '';
            } else {
                cardIcon.className = 'far fa-credit-card text-muted'; // Default
                cardIcon.style.color = '';
            }
        });
    }
});

// Medical Record Form Logic
document.addEventListener('DOMContentLoaded', function () {
    const statusSelect = document.getElementById('statusSelect');
    const injuryGroup = document.getElementById('injuryGroup');

    if (statusSelect && injuryGroup) {
        function toggleInjuryInput() {
            if (statusSelect.value === 'Injured') {
                injuryGroup.style.display = 'block';
            } else {
                injuryGroup.style.display = 'none';
            }
        }

        // Initial check
        toggleInjuryInput();

        // Listen for changes
        statusSelect.addEventListener('change', toggleInjuryInput);
    }
});

// Vaccination Form Logic
document.addEventListener('DOMContentLoaded', function () {
    const needsVaccinesYes = document.getElementById('needsVaccinesYes');
    const needsVaccinesNo = document.getElementById('needsVaccinesNo');
    const vaccineInputSection = document.getElementById('vaccineInputSection');
    const addVaccineBtn = document.getElementById('addVaccineBtn');
    const vaccineNameInput = document.getElementById('vaccineNameInput');
    const vaccineList = document.getElementById('vaccineList');
    const hiddenVaccinesContainer = document.getElementById('hiddenVaccinesContainer');
    const vaccineForm = document.getElementById('vaccineForm');

    if (needsVaccinesYes && needsVaccinesNo && vaccineInputSection) {
        function toggleVaccineSection() {
            if (needsVaccinesYes.checked) {
                vaccineInputSection.style.display = 'block';
            } else {
                vaccineInputSection.style.display = 'none';
            }
        }

        needsVaccinesYes.addEventListener('change', toggleVaccineSection);
        needsVaccinesNo.addEventListener('change', toggleVaccineSection);

        // Initial check
        toggleVaccineSection();
    }

    if (addVaccineBtn && vaccineNameInput && vaccineList && hiddenVaccinesContainer) {
        addVaccineBtn.addEventListener('click', function () {
            const vaccineName = vaccineNameInput.value.trim();
            if (vaccineName) {
                addVaccineItem(vaccineName);
                vaccineNameInput.value = '';
                vaccineNameInput.focus();
            }
        });

        // Allow Enter key to add
        vaccineNameInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                addVaccineBtn.click();
            }
        });
    }

    function addVaccineItem(name) {
        const li = document.createElement('li');
        li.className = 'list-group-item d-flex justify-content-between align-items-center';
        li.innerHTML = `
            <span>${name}</span>
            <button type="button" class="btn btn-danger btn-sm remove-vaccine">
                <i class="fas fa-times"></i>
            </button>
        `;

        // Add remove functionality
        li.querySelector('.remove-vaccine').addEventListener('click', function () {
            li.remove();
            updateHiddenInputs();
        });

        vaccineList.appendChild(li);
        updateHiddenInputs();
    }

    function updateHiddenInputs() {
        hiddenVaccinesContainer.innerHTML = '';
        const items = vaccineList.querySelectorAll('li span');
        items.forEach((item, index) => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `VaccineNames[${index}]`;
            input.value = item.textContent;
            hiddenVaccinesContainer.appendChild(input);
        });
    }
});

// Adoption Request Form Handler
document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("submit", function (event) {
        const form = event.target;

        // Handle Adopt Me form
        if (form.classList.contains('adopt-form')) {
            event.preventDefault();
            const formData = new FormData(form);
            const animalId = form.getAttribute('data-animal-id');
            const animalCard = document.getElementById('animal-card-' + animalId);

            fetch("/Request/Create", {
                method: "POST",
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showToast(data.message, "success");
                    if (animalCard) {
                        // On Index page - remove the card
                        animalCard.style.transition = 'opacity 0.5s, transform 0.5s';
                        animalCard.style.opacity = '0';
                        animalCard.style.transform = 'scale(0.8)';
                        setTimeout(() => animalCard.remove(), 500);
                    } else {
                        // On Details page - replace the form with success message
                        const actionCard = form.closest('.card-body');
                        if (actionCard) {
                            // Hide the form and any chat button
                            form.style.display = 'none';
                            const chatBtn = actionCard.querySelector('a[href*="Chat"]');
                            if (chatBtn) chatBtn.style.display = 'none';
                            
                            // Add success message
                            const successDiv = document.createElement('div');
                            successDiv.className = 'alert alert-success text-center rounded-3';
                            successDiv.innerHTML = '<i class="fas fa-check-circle me-2"></i>Adoption request sent! The owner will review your request.';
                            form.parentNode.insertBefore(successDiv, form);
                        }
                    }
                } else {
                    showToast(data.message || "Failed to send adoption request.", "error");
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showToast("An error occurred while sending the request.", "error");
            });
        }

        // Handle Complete Adoption form
        if (form.classList.contains('complete-adoption-form')) {
            event.preventDefault();
            const formData = new FormData(form);

            fetch("/Request/CompleteAdoption", {
                method: "POST",
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showToast(data.message, "success");
                    // Remove all request cards for this animal
                    const animalId = data.animalId;
                    const allCardsForAnimal = document.querySelectorAll(`.request-card[data-animal-id="${animalId}"]`);
                    allCardsForAnimal.forEach(card => {
                        card.style.transition = 'opacity 0.5s, transform 0.5s';
                        card.style.opacity = '0';
                        card.style.transform = 'scale(0.8)';
                        setTimeout(() => card.remove(), 500);
                    });
                } else {
                    showToast(data.message || "Failed to complete adoption.", "error");
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showToast("An error occurred while completing the adoption.", "error");
            });
        }
    });
});

// Notification Dropdown Functionality
document.addEventListener("DOMContentLoaded", function () {
    // Handle Notification Dropdown Open (Bootstrap Event)
    // Bootstrap 5 fires 'show.bs.dropdown' on the parent element of the toggle
    const dropdownToggle = document.getElementById('notificationDropdown');
    if (dropdownToggle && dropdownToggle.parentElement) {
        dropdownToggle.parentElement.addEventListener('show.bs.dropdown', function () {
            const list = $("#notification-list");
            list.html('<li><span class="dropdown-item text-muted text-center">Loading...</span></li>');

            $.get("/Chat/GetUserNotifications", function (data) {
                list.empty();
                if (data.length === 0) {
                    list.append('<li><span class="dropdown-item text-muted text-center">No new messages</span></li>');
                } else {
                    data.forEach(function (item) {
                        const html = `
                            <li>
                                <a class="dropdown-item d-flex justify-content-between align-items-center" href="/Chat/Index?receiverId=${item.userId}">
                                    <div class="d-flex align-items-center">
                                        <div class="bg-light rounded-circle d-flex align-items-center justify-content-center me-2" style="width: 40px; height: 40px;">
                                            <i class="fas fa-user text-secondary"></i>
                                        </div>
                                        <span class="fw-bold">${item.userName}</span>
                                    </div>
                                    <span class="badge bg-danger rounded-pill">${item.unreadCount}</span>
                                </a>
                            </li>
                        `;
                        list.append(html);
                    });
                }
            }).fail(function () {
                list.html('<li><span class="dropdown-item text-danger text-center">Error loading messages</span></li>');
            });
        });
    }
});

// Registration Form - Location Field Toggle
document.addEventListener('DOMContentLoaded', function () {
    const roleUser = document.getElementById('roleUser');
    const roleShelter = document.getElementById('roleShelter');
    const userLocationGroup = document.getElementById('userLocationGroup');
    const shelterLocationGroup = document.getElementById('shelterLocationGroup');
    const userLocationSelect = document.getElementById('userLocationSelect');
    const shelterLocationInput = document.getElementById('shelterLocationInput');

    // Only run if we're on the registration page
    if (roleUser && roleShelter && userLocationGroup && shelterLocationGroup) {
        function toggleLocationField() {
            if (roleShelter.checked) {
                userLocationGroup.style.display = 'none';
                shelterLocationGroup.style.display = 'block';
                if (userLocationSelect) userLocationSelect.removeAttribute('name');
                if (shelterLocationInput) {
                    shelterLocationInput.setAttribute('name', 'Location');
                    shelterLocationInput.required = true;
                }
                if (userLocationSelect) userLocationSelect.required = false;
            } else {
                userLocationGroup.style.display = 'block';
                shelterLocationGroup.style.display = 'none';
                if (userLocationSelect) userLocationSelect.setAttribute('name', 'Location');
                if (shelterLocationInput) shelterLocationInput.removeAttribute('name');
                if (userLocationSelect) userLocationSelect.required = false;
                if (shelterLocationInput) shelterLocationInput.required = false;
            }
        }

        roleUser.addEventListener('change', toggleLocationField);
        roleShelter.addEventListener('change', toggleLocationField);

        // Initial check
        toggleLocationField();
    }
});

// Animal Breed Selection Logic
document.addEventListener('DOMContentLoaded', function () {
    // Breeds data for each animal type
    const breedsData = {
        'Dog': ['Golden Retriever', 'Labrador Retriever', 'German Shepherd', 'Bulldog', 'Poodle', 'Beagle', 'Husky', 'Rottweiler', 'Boxer', 'Dachshund', 'Great Dane', 'Doberman', 'Shih Tzu', 'Other'],
        'Cat': ['Persian', 'Siamese', 'Maine Coon', 'British Shorthair', 'Bengal', 'Ragdoll', 'Sphynx', 'Abyssinian', 'Scottish Fold', 'Russian Blue', 'Other'],
        'Bird': ['Parrot', 'Canary', 'Cockatiel', 'Budgie', 'Finch', 'Lovebird', 'Macaw', 'Cockatoo', 'African Grey', 'Parakeet', 'Other'],
        'Rabbit': ['Holland Lop', 'Mini Rex', 'Lionhead', 'Dutch', 'Flemish Giant', 'Netherland Dwarf', 'Mini Lop', 'Rex', 'English Angora', 'Other'],
        'Hamster': ['Syrian', 'Dwarf Campbell', 'Dwarf Winter White', 'Roborovski', 'Chinese', 'Other'],
        'Fish': ['Goldfish', 'Betta', 'Guppy', 'Angelfish', 'Neon Tetra', 'Molly', 'Oscar', 'Discus', 'Koi', 'Other'],
        'Turtle': ['Red-Eared Slider', 'Box Turtle', 'Painted Turtle', 'Snapping Turtle', 'Map Turtle', 'Musk Turtle', 'Softshell Turtle', 'Other']
    };

    const animalTypeSelect = document.getElementById('animalType');
    const breedGroup = document.getElementById('breedGroup');
    const breedSelect = document.getElementById('breedSelect');
    const customBreedGroup = document.getElementById('customBreedGroup');
    const customBreedInput = document.getElementById('customBreedInput');

    // Only run if we're on the animal create page
    if (animalTypeSelect && breedGroup && breedSelect) {
        // Handle animal type change
        animalTypeSelect.addEventListener('change', function () {
            const selectedType = this.value;

            // Clear previous selections
            breedSelect.innerHTML = '<option value="">-- Select Breed --</option>';
            if (customBreedInput) customBreedInput.value = '';
            if (customBreedGroup) customBreedGroup.style.display = 'none';

            if (selectedType && selectedType !== 'Other') {
                // Show breed dropdown for known types
                breedGroup.style.display = 'block';

                const breeds = breedsData[selectedType] || [];
                breeds.forEach(breed => {
                    const option = document.createElement('option');
                    option.value = breed;
                    option.textContent = breed;
                    breedSelect.appendChild(option);
                });
            } else if (selectedType === 'Other') {
                // For "Other" type, show custom breed input directly
                breedGroup.style.display = 'none';
                if (customBreedGroup) customBreedGroup.style.display = 'block';
            } else {
                breedGroup.style.display = 'none';
            }
        });

        // Handle breed selection change
        breedSelect.addEventListener('change', function () {
            const selectedBreed = this.value;

            if (selectedBreed === 'Other') {
                if (customBreedGroup) customBreedGroup.style.display = 'block';
            } else {
                if (customBreedGroup) customBreedGroup.style.display = 'none';
                if (customBreedInput) customBreedInput.value = '';
            }
        });
    }
});